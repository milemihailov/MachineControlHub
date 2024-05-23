using MachineControlHub.Print;
using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MudBlazor;
using WebUI.Pages;
using Microsoft.AspNetCore.Components;
using System;
using MachineControlHub;
using static MudBlazor.Colors;
using static MachineControlHub.Print.PrintProgress;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 90);
        const string PATTERN = @"echo: M73 Time left: ((\d+h\s*)?(\d+m\s*)?(\d+s)?);";

        public PrintService printService;
        public CurrentPrintJob printJob;
        public PrintProgress printProgress;
        public BackgroundTimer background;
        public readonly IDialogService _dialogService;
        public readonly ISnackbar _snackbar;

        public string printName;
        public string uploadFileName;
        public TimeSpan estimatedTime;
        public double fileSize;
        public string extractedSettings;
        public string timeElapsed;
        public List<string> files;
        public List<(string FileName, string FileContent, long FileSize)> uploadedFiles = new List<(string FileName, string FileContent, long FileSize)>();
        public string file;
        public string fileToPrint = "";
        public bool _processing = false;
        public double progress = 0;
        public bool _isPrinting = false;
        public bool autoReportTemps { get; set; } = true;
        public int? autoReportValueInterval { get; set; }

        public static List<double> hotendGraph = new List<double> { };
        public static List<double> bedGraph = new List<double> { };
        public ChartOptions Options = new ChartOptions();
        public string[] XAxisLabels = { };
        public int Index = -1;



        public PrintingService(IDialogService dialogService, ISnackbar snackbar, BackgroundTimer background)
        {
            this.background = background;
            printService = new PrintService(background.ConnectionServiceSerial.printerConnection);
            printJob = new CurrentPrintJob(background.ConnectionServiceSerial.printerConnection);
            printProgress = new PrintProgress();
            _dialogService = dialogService;
            _snackbar = snackbar;
            _snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        }

        public void StartPrint(string fileName)
        {
            printService.StartPrint(fileName);
        }

        public void PausePrint()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildPauseSDPrintCommand());
            _snackbar.Add("Print Paused", Severity.Info);
        }

        public void ResumePrint()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildStartSDPrintCommand());
            _snackbar.Add($"<ul><li>Waiting for temperature</li><li>Print Resuming</li></ul>", Severity.Info);
        }

        public async Task StopPrint()
        {
            if (background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                bool? result = await _dialogService.ShowMessageBox(
                "Stop Print",
                "Do you want to stop the print?",
                yesText: "Stop!", cancelText: "Cancel");

                if (result == true && _isPrinting)
                {
                    printService.AbortCurrentPrint();
                    progress = 0;
                    _isPrinting = false;
                    _snackbar.Add("Print Stopped", Severity.Error);
                }
                else
                {
                    _snackbar.Add("Not Printing", Severity.Error);
                    return;
                }
            }
            else
            {
                _snackbar.Add("Printer is not connected", Severity.Error);
                return;
            }
        }

        public void ListSDFiles(string inputText)
        {
            files = printService.ListSDFiles(inputText);
        }

        public void StartTimeOfPrint()
        {
            printJob.ParseStartTimeOfPrint();
        }

        public void ExtractPrintingSettings(string file)
        {
            printJob.ParseExtractedSettingsFromPrintedFile(file);
            extractedSettings = printJob.ExtractedSettingsFromPrintedFile;
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileNameAndSize(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB, 2);
        }


        public void ElapsedPrintTime(string message)
        {
            if (message.Contains("echo:Print time:"))
            {
                Match match = Regex.Match(message, @"(?<=echo:Print time: )((\d+h\s*)?(\d+m\s*)?(\d+s)?)", RegexOptions.IgnoreCase);
                string newTimeElapsed = match.Groups[0].Value;

                if (newTimeElapsed != timeElapsed)
                {
                    timeElapsed = newTimeElapsed;
                }
            }
        }


        public void StartPrintTimer()
        {
            background.ConnectionServiceSerial.Write("M75");
        }

        public void PausePrintTimer()
        {
            background.ConnectionServiceSerial.Write("M76");
        }

        public void StopPrintTimer()
        {
            if (_isPrinting)
            {
                background.ConnectionServiceSerial.Write("M77");
            }
        }

        public async Task ConfirmStartAsync()
        {
            if (background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                bool? result = await _dialogService.ShowMessageBox(
                "Start Print",
                "Do you want to start a print job?",
                yesText: "Start!", cancelText: "Cancel");

                if (result == true)
                {
                    if (fileToPrint == "")
                    {
                        _snackbar.Add("No file selected", Severity.Error);
                    }
                    else
                    {
                        StartPrint(fileToPrint);
                        StartPrintTimer();
                        StartTimeOfPrint();
                        GetFileNameAndSize(fileToPrint);
                        _snackbar.Add($"<ul><li>Print Started</li> <li> File Printing: {fileToPrint} </li></ul>", Severity.Success);
                    }
                }
            }
            else
            {
                _snackbar.Add("Printer is not connected", Severity.Error);
                return;
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


        public async void UpdatePrintProgress(string message)
        {
            if (progress > 0)
            {
                _isPrinting = true;
                if (progress == 100)
                {
                    _isPrinting = false;
                    progress = 0;
                }
            }
            await Task.Run(() =>
            {
                var printing = Regex.Match(message, @"printing byte (\d+)/(\d+)");
                var finished = Regex.Match(message, @"Done printing file");

                if (printing.Success)
                {
                    long bytesPrinted = int.Parse(printing.Groups[1].Value);
                    printProgress.TotalBytes = int.Parse(printing.Groups[2].Value);
                    printProgress.PrintProgressRecords.Add(new PrintProgressRecord { BytesPrinted = bytesPrinted, Timestamp = DateTime.Now });
                    if (printProgress.PrintProgressRecords.Count > 200) // Keep the last 5 records
                    {
                        printProgress.PrintProgressRecords.RemoveAt(0);
                    }
                    progress = Math.Round(CalculatePercentage(bytesPrinted, printProgress.TotalBytes));
                }

                if (finished.Success)
                {
                    progress = 100;
                    _snackbar.Add("Print Finished", Severity.Success);
                }
            });
        }


        public void DisplayEstimatedTimeRemaining()
        {
            estimatedTime = printProgress.EstimateTimeRemaining();
        }

        public void CalibrateBed()
        {
            _processing = true;
            background.ConnectionServiceSerial.Write(CommandMethods.BuildBedLevelingCommand());
        }

        public List<(string DriveName, string VolumeLabel)> PortsAvailable = new List<(string DriveName, string VolumeLabel)>();
        public string chosenPort = "";
        public bool isTransferring = false;
        public bool mediaAttached = true;

        public async Task WriteFileToPort(string driveName, string fileName)
        {
            isTransferring = true;

            string filePath = Path.Combine(driveName, fileName);

            await Task.Run(() => File.WriteAllText(filePath, file));

            isTransferring = false;
            _snackbar.Add("File transferred to media", Severity.Success);
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
            background.ConnectionServiceSerial.printerConnection.Write("M22");
            mediaAttached = false;
        }

        public void AttachMedia()
        {
            background.ConnectionServiceSerial.printerConnection.Write("M21");
            mediaAttached = true;
        }
    }
}
