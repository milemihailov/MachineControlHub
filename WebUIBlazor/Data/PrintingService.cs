using MachineControlHub.Print;
using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MudBlazor;
using WebUI.Pages;
using Microsoft.AspNetCore.Components;
using System;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 30);
        const string PATTERN = @"echo: M73 Time left: ((\d+h\s*)?(\d+m\s*)?(\d+s)?);";

        public PrintService printService;
        public CurrentPrintJob printJob;
        public PrintProgress printProgress;
        public readonly IDialogService _dialogService;
        public readonly ISnackbar _snackbar;

        public string printName;
        public string uploadFileName;
        public string estimatedTime;
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

        public static List<double> hotendGraph = new List<double> { };
        public static List<double> bedGraph = new List<double> { };
        public ChartOptions Options = new ChartOptions();
        public string[] XAxisLabels = { };
        public int Index = -1;



        public PrintingService(IDialogService dialogService, ISnackbar snackbar)
        {
            printService = new PrintService(ConnectionServiceSerial.printerConnection);
            printJob = new CurrentPrintJob(ConnectionServiceSerial.printerConnection);
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
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPauseSDPrintCommand());
            _snackbar.Add("Print Paused", Severity.Info);
        }

        public void ResumePrint()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildStartSDPrintCommand());
            _snackbar.Add($"<ul><li>Waiting for temperature</li><li>Print Resuming</li></ul>", Severity.Info);
        }

        public void StopPrint()
        {
            printService.AbortCurrentPrint();
            progress = 0;
            _isPrinting = false;
            _snackbar.Add("Print Stopped", Severity.Error);
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

        public void TransferFileToSD(string file, string fileName)
        {
            //printService.TransferFileToSD(file, fileName);
            throw new NotImplementedException();
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileNameAndSize(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB, 2);
        }

        public void EstimatedPrintTime(string message)
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPrintProgressCommand());
            Match match = Regex.Match(message, PATTERN, RegexOptions.IgnoreCase);
            estimatedTime = match.Groups[1].Value;
        }

        public async Task ConfirmStartAsync()
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
                    StartTimeOfPrint();
                    GetFileNameAndSize(fileToPrint);
                    _snackbar.Add($"<ul><li>Print Started</li> <li> File Printing: {fileToPrint} </li></ul>", Severity.Success);
                }
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

        public static double CalculatePercentage(double numerator, double denominator)
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
                    int firstNumber = int.Parse(printing.Groups[1].Value);
                    int secondNumber = int.Parse(printing.Groups[2].Value);
                    progress = Math.Round(CalculatePercentage(firstNumber, secondNumber));
                }

                if(finished.Success)
                {
                    progress = 100;
                }
            });
        }

        public void CalibrateBed()
        {
            _processing = true;
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildBedLevelingCommand());
        }
    }
}
