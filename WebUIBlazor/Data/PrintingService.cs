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
        public BackgroundTimer background;
        public readonly IDialogService _dialogService;
        public readonly ISnackbar _snackbar;
        public readonly PrinterDataServiceTest _printerDataServiceTest;

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
        public bool _processing = false;
        public double progress = 0;
        public bool _isPrinting = false;

        public ChartOptions Options = new ChartOptions();
        public string[] XAxisLabels = Array.Empty<string>();
        public int Index = -1;



        public PrintingService(IDialogService dialogService, ISnackbar snackbar, BackgroundTimer background, PrinterDataServiceTest printerDataServiceTest)
        {
            this.background = background;
            _printerDataServiceTest = printerDataServiceTest;
            printService = new PrintService(background.ConnectionServiceSerial.printerConnection);
            printJob = new CurrentPrintJob(background.ConnectionServiceSerial.printerConnection);
            _dialogService = dialogService;
            _snackbar = snackbar;
            _snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            _printerDataServiceTest = printerDataServiceTest;
        }

        public void StartPrint(string fileName)
        {
            printService.StartPrint(fileName);
            printJob.PrintProgressRecords.Clear();
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
                    background.StopStopwatch();
                    progress = 0;
                    FormatTotalPrintTime();
                    _isPrinting = false;
                    _printerDataServiceTest.AddPrintJobToHistory(printJob);

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
            if (_printerDataServiceTest.Printer.HasLongFilenameSupport)
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

        //public void ExtractPrintingSettings(string file)
        //{
        //    printJob.ParseExtractedSettingsFromPrintedFile(file);
        //    extractedSettings = printJob.ExtractedSettingsFromPrintedFile;
        //}

        public void GetFileNameAndSize(string input)
        {
            var file = SDFiles.FirstOrDefault(f => f.FileName == input);
            if (file != default)
            {
                printJob.FileName = file.FileName;
                printJob.FileSize = double.Parse(file.FileSize);
            }
        }


        //public void ElapsedPrintTime(string message)
        //{
        //    if (message.Contains("echo:Print time:"))
        //    {
        //        Match match = Regex.Match(message, @"(?<=echo:Print time: )((\d+h\s*)?(\d+m\s*)?(\d+s)?)", RegexOptions.IgnoreCase);
        //        string newTimeElapsed = match.Groups[0].Value;

        //        if (newTimeElapsed != timeElapsed)
        //        {
        //            timeElapsed = newTimeElapsed;
        //        }
        //    }
        //}

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
                        //StartPrintTimer();
                        StartTimeOfPrint();
                        background.ResetStopwatch();
                        background.StartStopwatch();
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
                    background.StopStopwatch();
                    progress = 0;
                    FormatTotalPrintTime();
                    _printerDataServiceTest.AddPrintJobToHistory(printJob);
                }
            }
            await Task.Run(() =>
            {
                var printing = Regex.Match(message, @"printing byte (\d+)/(\d+)");
                var finished = Regex.Match(message, @"Done printing file");

                if (printing.Success)
                {
                    printJob.CurrentBytes = int.Parse(printing.Groups[1].Value);
                    printJob.TotalBytes = int.Parse(printing.Groups[2].Value);
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
                    _snackbar.Add("Print Finished", Severity.Success);
                }
            });
        }


        public async Task DisplayEstimatedTimeRemaining()
        {
            estimatedTime = await printJob.EstimateTimeRemainingAsync();
        }

        public void CalibrateBed()
        {
            _processing = true;
            background.ConnectionServiceSerial.Write(CommandMethods.BuildBedLevelingCommand());
            Console.WriteLine("starting");
        }

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
