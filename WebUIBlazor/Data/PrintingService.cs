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

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 90);
        const string _pATTERN = @"echo: M73 Time left: ((\d+h\s*)?(\d+m\s*)?(\d+s)?);";

        public BackgroundTimer Background { get; set; }
        public PrinterDataService PrinterDataServiceTest { get; set; }
        public PrinterManagerService PrinterManager { get; set; }

        public List<(string DriveName, string VolumeLabel)> PortsAvailable { get; set; } = new List<(string DriveName, string VolumeLabel)>();
        public List<(string FileName, string FileContent, long FileSize)> UploadedFiles { get; set; } = new List<(string FileName, string FileContent, long FileSize)>();
        public List<(string FileName, string FileSize)> SDFiles { get; set; }
        public static List<double> HotendGraph { get; set; } = new() { };
        public static List<double> BedGraph { get; set; } = new() { };
        public string ChosenPort { get; set; } = "";
        public bool IsTransferring { get; set; } = false;
        public bool MediaAttached { get; set; } = true;
        public TimeSpan EstimatedTime { get; set; }
        public string File { get; set; }
        public string FileToPrint { get; set; } = "";
        public double Progress { get; set; } = 0;
        public ChartOptions Options { get; set; } = new ChartOptions();
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();
        public int Index { get; set; } = -1;
        public bool FinalizationExecuted { get; set; } = false;



        public PrintingService(PrinterDataService printerDataServiceTest, BackgroundTimer background, PrinterManagerService printerManager)
        {
            PrinterManager = printerManager;
            PrinterManager.ActivePrinter = new();
            PrinterDataServiceTest = printerDataServiceTest;
            PrinterDataServiceTest = printerDataServiceTest;
            Background = background;
            PrinterManager = printerManager;
        }

        public void StartPrint(string fileName)
        {
            PrinterManager.ActivePrinter.PrintService.StartPrint(fileName);
            PrinterManager.ActivePrinter.CurrentPrintJob.PrintProgressRecords.Clear();
        }

        public void PausePrint()
        {
            PrinterManager.ActivePrinter.SerialConnection.Write(CommandMethods.BuildPauseSDPrintCommand());
        }

        public void ResumePrint()
        {
            PrinterManager.ActivePrinter.SerialConnection.Write(CommandMethods.BuildStartSDPrintCommand());
        }


        public void ListSDFiles(string inputText)
        {
            if (PrinterManager.ActivePrinter.HasLongFilenameSupport)
            {
                SDFiles = PrinterManager.ActivePrinter.PrintService.ListLongNameSDFiles(inputText);
            }
            else
            {
                SDFiles = PrinterManager.ActivePrinter.PrintService.ListSDFiles(inputText);
            }
        }

        public void StartTimeOfPrint()
        {
            PrinterManager.ActivePrinter.CurrentPrintJob.ParseStartTimeOfPrint();
        }

        public void FormatTotalPrintTime()
        {
            TimeSpan elapsed = TimeSpan.FromMilliseconds(Background.stopwatch.ElapsedMilliseconds);
            PrinterManager.ActivePrinter.CurrentPrintJob.TotalPrintTime = string.Format($"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}");
        }

        public void GetFileNameAndSize(string input)
        {
            var file = SDFiles.FirstOrDefault(f => f.FileName == input);
            if (file != default)
            {
                PrinterManager.ActivePrinter.CurrentPrintJob.FileName = file.FileName;
                PrinterManager.ActivePrinter.CurrentPrintJob.FileSize = double.Parse(file.FileSize);
            }
        }


        public List<ChartSeries> Series = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "Hotend", Data = HotendGraph.ToArray() },
        new ChartSeries() { Name = "Bed", Data = BedGraph.ToArray() },
    };

        public void UpdateGraphData()
        {
            var new_series = new List<ChartSeries>()
        {
            new ChartSeries() { Name = "Hotend", Data = HotendGraph.ToArray() },
            new ChartSeries() { Name = "Bed", Data = BedGraph.ToArray() },
        };
            Series = new_series;
        }

        public double CalculatePercentage(double numerator, double denominator)
        {
            double fraction = numerator / denominator;
            return fraction * 100;
        }


        public void UpdatePrintProgress(string message)
        {
            var printing = Regex.Match(message, @"printing byte (\d+)/(\d+)");
            var finished = Regex.Match(message, @"Done printing file");
            var notPrinting = Regex.Match(message.Trim(), @"\s*Not\s+SD\s*", RegexOptions.IgnoreCase);


            if (printing.Success)
            {
                PrinterManager.ActivePrinter.CurrentPrintJob.CurrentBytes = int.Parse(printing.Groups[1].Value);
                PrinterManager.ActivePrinter.CurrentPrintJob.TotalBytes = int.Parse(printing.Groups[2].Value);
                PrinterManager.ActivePrinter.CurrentPrintJob.FileSize = PrinterManager.ActivePrinter.CurrentPrintJob.TotalBytes;
                PrinterManager.ActivePrinter.CurrentPrintJob.PrintProgressRecords.Add(new PrintProgressRecord { BytesPrinted = PrinterManager.ActivePrinter.CurrentPrintJob.CurrentBytes, Timestamp = DateTime.Now });
                if (PrinterManager.ActivePrinter.CurrentPrintJob.PrintProgressRecords.Count > 100) // Keep the last 100 records
                {
                    PrinterManager.ActivePrinter.CurrentPrintJob.PrintProgressRecords.RemoveAt(0);
                }
                Progress = Math.Round(CalculatePercentage(PrinterManager.ActivePrinter.CurrentPrintJob.CurrentBytes, PrinterManager.ActivePrinter.CurrentPrintJob.TotalBytes));
                PrinterManager.ActivePrinter.CurrentPrintJob.IsPrinting = true;
            }

            if (notPrinting.Success)
            {
                PrinterManager.ActivePrinter.CurrentPrintJob.IsPrinting = false;
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
                    PrinterManager.ActivePrinter.CurrentPrintJob.IsPrinting = true;
                }

                if (Progress == 100 && !FinalizationExecuted)
                {
                    PrinterManager.ActivePrinter.CurrentPrintJob.IsPrinting = false;
                    Background.StopStopwatch();
                    Progress = 0;
                    FormatTotalPrintTime();
                    PrinterDataServiceTest.AddPrintJobToHistory(PrinterManager.ActivePrinter.CurrentPrintJob, PrinterManager);
                    FinalizationExecuted = true;
                }
            }

        }

        public void GetPrintingFileName(string fileName)
        {
            var printFile = Regex.Match(fileName, @"Current file: (.*)");
            try
            {
                if (printFile.Success)
                {
                    PrinterManager.ActivePrinter.CurrentPrintJob.FileName = printFile.Groups[1].Value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task DisplayEstimatedTimeRemaining()
        {
            EstimatedTime = await PrinterManager.ActivePrinter.CurrentPrintJob.EstimateTimeRemainingAsync();
        }

        public async Task WriteFileToPort(string driveName, string fileName)
        {
            IsTransferring = true;

            string filePath = Path.Combine(driveName, fileName);

            await Task.Run(() => System.IO.File.WriteAllText(filePath, File));

            IsTransferring = false;
        }


        public void ChoosePort(string portname)
        {
            ChosenPort = portname;
        }

        public void GetDrives()
        {
            PortsAvailable.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    PortsAvailable.Add((drive.Name, drive.VolumeLabel));
                }
            }
        }

        public void ReleaseMedia()
        {
            PrinterManager.ActivePrinter.SerialConnection.Write("M22");
            MediaAttached = false;
        }

        public void AttachMedia()
        {
            PrinterManager.ActivePrinter.SerialConnection.Write("M21");
            MediaAttached = true;
        }
    }
}