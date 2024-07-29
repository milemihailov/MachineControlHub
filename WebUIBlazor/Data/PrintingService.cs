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

        public PrintService printService;
        public CurrentPrintJob printJob;
        public PortConnectionManagerService portConnectionManager;
        public BackgroundTimer background;
        public readonly IDialogService _dialogService;
        public readonly ISnackbar _snackbar;
        public PrinterDataService _printerDataServiceTest;

        public List<(string DriveName, string VolumeLabel)> PortsAvailable = new List<(string DriveName, string VolumeLabel)>();
        public List<(string FileName, string FileContent, long FileSize)> uploadedFiles = new List<(string FileName, string FileContent, long FileSize)>();
        public List<(string FileName, string FileSize)> SDFiles;
        public static List<double> hotendGraph = new List<double> { };
        public static List<double> bedGraph = new List<double> { };
        public string chosenPort = "";
        public bool isTransferring = false;
        public bool mediaAttached = true;
        public string uploadFileName;
        public TimeSpan estimatedTime;
        public string extractedSettings;
        public string timeElapsed;
        public string file;
        public string fileToPrint = "";
        public bool processing = false;
        public double progress = 0;
        public bool isPrinting = false;

        public ChartOptions Options = new ChartOptions();
        public string[] XAxisLabels = Array.Empty<string>();
        public int Index = -1;
        public bool finalizationExecuted = false;



        public PrintingService(PortConnectionManagerService portConnectionManager, PrinterDataService printerDataServiceTest, BackgroundTimer background)
        {
            this.portConnectionManager = portConnectionManager;
            _printerDataServiceTest = printerDataServiceTest;
            printService = new PrintService(portConnectionManager.ActiveConnection.ConnectionServiceSerial.printerConnection);
            printJob = new CurrentPrintJob(portConnectionManager.ActiveConnection.ConnectionServiceSerial.printerConnection);
            _printerDataServiceTest = printerDataServiceTest;
            this.background = background;
        }

        public void StartPrint(string fileName)
        {
            printService.StartPrint(fileName);
            printJob.PrintProgressRecords.Clear();
        }

        public void PausePrint()
        {
            portConnectionManager.ActiveConnection.ConnectionServiceSerial.Write(CommandMethods.BuildPauseSDPrintCommand());
            //_snackbar.Add("Print Paused", Severity.Info);
        }

        public void ResumePrint()
        {
            portConnectionManager.ActiveConnection.ConnectionServiceSerial.Write(CommandMethods.BuildStartSDPrintCommand());
            //_snackbar.Add($"<ul><li>Waiting for temperature</li><li>Print Resuming</li></ul>", Severity.Info);
        }


        public void ListSDFiles(string inputText)
        {
            if (_printerDataServiceTest.ActivePrinter.HasLongFilenameSupport)
            {
                SDFiles = printService.ListLongNameSDFiles(inputText);
            }
            else
            {
                SDFiles = printService.ListSDFiles(inputText);
            }
        }

        public void StartTimeOfPrint()
        {
            printJob.ParseStartTimeOfPrint();
        }

        public void FormatTotalPrintTime()
        {
            TimeSpan elapsed = TimeSpan.FromMilliseconds(_printerDataServiceTest.Background.stopwatch.ElapsedMilliseconds);
            printJob.TotalPrintTime = string.Format($"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}");
        }

        public void GetFileNameAndSize(string input)
        {
            var file = SDFiles.FirstOrDefault(f => f.FileName == input);
            if (file != default)
            {
                printJob.FileName = file.FileName;
                printJob.FileSize = double.Parse(file.FileSize);
            }
        }


        public List<ChartSeries> Series = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "Hotend", Data = hotendGraph.ToArray() },
        new ChartSeries() { Name = "Bed", Data = bedGraph.ToArray() },
    };

        public void UpdateGraphData()
        {
            var new_series = new List<ChartSeries>()
        {
            new ChartSeries() { Name = "Hotend", Data = hotendGraph.ToArray() },
            new ChartSeries() { Name = "Bed", Data = bedGraph.ToArray() },
        };
            Series = new_series;
        }

        public double CalculatePercentage(double numerator, double denominator)
        {
            double fraction = numerator / denominator;
            return fraction * 100;
        }


        public void UpdatePrintProgress(string message, SerialDataProcessorService source)
        {
            var printing = Regex.Match(message, @"printing byte (\d+)/(\d+)");
            var finished = Regex.Match(message, @"Done printing file");

            if (printing.Success)
            {
                printJob.CurrentBytes = int.Parse(printing.Groups[1].Value);
                printJob.TotalBytes = int.Parse(printing.Groups[2].Value);
                printJob.FileSize = printJob.TotalBytes;
                printJob.PrintProgressRecords.Add(new PrintProgressRecord { BytesPrinted = printJob.CurrentBytes, Timestamp = DateTime.Now });
                if (printJob.PrintProgressRecords.Count > 100) // Keep the last 100 records
                {
                    printJob.PrintProgressRecords.RemoveAt(0);
                }
                progress = Math.Round(CalculatePercentage(printJob.CurrentBytes, printJob.TotalBytes));
            }

            if (finished.Success)
            {
                progress = 100;
                //_snackbar.Add("Print Finished", Severity.Success);
            }

            if (progress > 0)
            {
                if (!finalizationExecuted)
                {
                    isPrinting = true;
                }

                if (progress == 100 && !finalizationExecuted)
                {
                    isPrinting = false;
                    background.StopStopwatch();
                    progress = 0;
                    FormatTotalPrintTime();
                    _printerDataServiceTest.AddPrintJobToHistory(printJob, source);
                    finalizationExecuted = true;
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
                    printJob.FileName = printFile.Groups[1].Value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task DisplayEstimatedTimeRemaining()
        {
            estimatedTime = await printJob.EstimateTimeRemainingAsync();
        }

        public void CalibrateBed()
        {
            processing = true;
            portConnectionManager.connections[portConnectionManager.SelectedPrinter].ConnectionServiceSerial.Write(CommandMethods.BuildBedLevelingCommand());
            Console.WriteLine("starting");
        }

        public async Task WriteFileToPort(string driveName, string fileName)
        {
            isTransferring = true;

            string filePath = Path.Combine(driveName, fileName);

            await Task.Run(() => File.WriteAllText(filePath, file));

            isTransferring = false;
            //_snackbar.Add("File transferred to media", Severity.Success);
        }


        public void ChoosePort(string portname)
        {
            chosenPort = portname;
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
            portConnectionManager.ActiveConnection.ConnectionServiceSerial.Write("M22");
            mediaAttached = false;
        }

        public void AttachMedia()
        {
            portConnectionManager.ActiveConnection.ConnectionServiceSerial.Write("M21");
            mediaAttached = true;
        }
    }
}