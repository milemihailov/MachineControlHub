using MachineControlHub.Print;
using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MudBlazor;
using WebUI.Pages;
using Microsoft.AspNetCore.Components;
using System;
using MachineControlHub;
using static MudBlazor.Colors;
using static MachineControlHub.Print.CurrentPrintJob;
using System.Reflection;
using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 90);

        public BackgroundTimer Background { get; set; }
        public PrinterDataService PrinterDataServiceTest { get; set; }

        public List<(string DriveName, string VolumeLabel)> DriversAvailable { get; set; } = new List<(string DriveName, string VolumeLabel)>();
        public List<(string FileName, string FileContent, long FileSize)> UploadedFiles { get; set; } = new List<(string FileName, string FileContent, long FileSize)>();
        public List<(string FileName, string FileSize)> SDFiles { get; set; }
        public static List<double> HotendGraph { get; set; } = new() { };
        public static List<double> BedGraph { get; set; } = new() { };
        public string DriveLetter { get; set; } = null;
        public bool IsTransferring { get; set; } = false;
        public bool MediaAttached { get; set; } = true;
        public string File { get; set; }
        public string FileToPrint { get; set; } = "";
        public double Progress { get; set; } = 0;
        public ChartOptions Options { get; set; } = new ChartOptions();
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();
        public int Index { get; set; } = -1;
        public bool FinalizationExecuted { get; set; } = false;



        public PrintingService(PrinterDataService printerDataServiceTest, BackgroundTimer background)
        {
            PrinterDataServiceTest = printerDataServiceTest;
            Background = background;
        }

        public void StartPrint(string fileName, Printer printer)
        {
            printer.PrintService.StartPrint(fileName);
            printer.CurrentPrintJob.PrintProgressRecords.Clear();
        }

        public void PausePrint(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildPauseSDPrintCommand());
        }

        public void ResumePrint(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildStartSDPrintCommand());
        }

        public void CancelCurrentObject(Printer printer)
        {
            printer.SerialConnection.Write("M486 C");
        }

        public void ListSDFiles(string inputText, Printer printer)
        {
            if (printer.HasLongFilenameSupport)
            {
                SDFiles = printer.PrintService.ListLongNameSDFiles(inputText);
            }
            else
            {
                SDFiles = printer.PrintService.ListSDFiles(inputText);
            }
        }

        public void StartTimeOfPrint(Printer printer)
        {
            printer.CurrentPrintJob.ParseStartTimeOfPrint();
        }

        public void FormatTotalPrintTime(Printer printer)
        {
            TimeSpan elapsed = TimeSpan.FromMilliseconds(Background.stopwatch.ElapsedMilliseconds);
            printer.CurrentPrintJob.TotalPrintTime = string.Format($"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}");
        }

        public void GetFileNameAndSize(string input, Printer printer)
        {
            var file = SDFiles.FirstOrDefault(f => f.FileName == input);
            if (file != default)
            {
                printer.CurrentPrintJob.FileName = file.FileName;
                printer.CurrentPrintJob.FileSize = double.Parse(file.FileSize);
            }
        }


        public List<ChartSeries> Series = new()
    {
        new() { Name = "Hotend", Data = HotendGraph.ToArray() },
        new() { Name = "Bed", Data = BedGraph.ToArray() },
    };

        public void UpdateGraphData()
        {
            var new_series = new List<ChartSeries>()
        {
            new() { Name = "Hotend", Data = HotendGraph.ToArray() },
            new() { Name = "Bed", Data = BedGraph.ToArray() },
        };
            Series = new_series;
        }

        public double CalculatePercentage(double numerator, double denominator)
        {
            double fraction = numerator / denominator;
            return fraction * 100;
        }


        public void UpdatePrintProgress(string message, Printer printer)
        {
            var printing = Regex.Match(message, @"printing byte (\d+)/(\d+)");
            var finished = Regex.Match(message, @"Done printing file");
            var notPrinting = Regex.Match(message.Trim(), @"\s*Not\s+SD\s*", RegexOptions.IgnoreCase);


            if (printing.Success)
            {
                printer.CurrentPrintJob.CurrentBytes = int.Parse(printing.Groups[1].Value);
                printer.CurrentPrintJob.TotalBytes = int.Parse(printing.Groups[2].Value);
                printer.CurrentPrintJob.FileSize = printer.CurrentPrintJob.TotalBytes;
                printer.CurrentPrintJob.PrintProgressRecords.Add(new PrintProgressRecord { BytesPrinted = printer.CurrentPrintJob.CurrentBytes, Timestamp = DateTime.Now });

                if (printer.CurrentPrintJob.PrintProgressRecords.Count > 100) // Keep the last 100 records
                {
                    printer.CurrentPrintJob.PrintProgressRecords.RemoveAt(0);
                }
                Progress = Math.Round(CalculatePercentage(printer.CurrentPrintJob.CurrentBytes, printer.CurrentPrintJob.TotalBytes));
                printer.CurrentPrintJob.IsPrinting = true;
            }

            if (notPrinting.Success)
            {
                printer.CurrentPrintJob.IsPrinting = false;
                Progress = 0;
            }

            if (finished.Success)
            {
                Progress = 100;
            }

            if (Progress > 0)
            {
                if (!FinalizationExecuted)
                {
                    printer.CurrentPrintJob.IsPrinting = true;
                }

                if (Progress == 100 && !FinalizationExecuted)
                {
                    printer.CurrentPrintJob.IsPrinting = false;
                    printer.CurrentPrintJob.StopStopwatch();
                    Progress = 0;
                    FormatTotalPrintTime(printer);
                    PrinterDataServiceTest.AddPrintJobToHistory(printer);
                    FinalizationExecuted = true;
                }
            }
        }

        public void GetPrintingFileName(string fileName, Printer printer)
        {
            var printFile = Regex.Match(fileName, @"Current file: (.*)");
            try
            {
                if (printFile.Success)
                {
                    printer.CurrentPrintJob.FileName = printFile.Groups[1].Value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task WriteFileToDrive(string driveName, string fileName, Printer printer)
        {
            printer.IsTransferringFile = true;

            string filePath = Path.Combine(driveName, fileName);

            await Task.Run(() => System.IO.File.WriteAllText(filePath, File));

            printer.IsTransferringFile = false;
        }

        public void GetDrives()
        {
            DriversAvailable.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    DriversAvailable.Add((drive.Name, drive.VolumeLabel));
                }
            }
        }

        public void ReleaseMedia(Printer printer)
        {
            printer.SerialConnection.Write("M22");
            DriveLetter = null;
            SDFiles.Clear();
            printer.MediaAttached = false;
        }

        public void AttachMedia(Printer printer)
        {
            printer.SerialConnection.Write("M21");
            DriveLetter = null;
            printer.MediaAttached = true;
        }
    }
}