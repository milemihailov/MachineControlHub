using MachineControlHub.Print;
using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MudBlazor;
using WebUI.Pages;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 30);
        const string PATTERN = @"echo: M73 Time left: ((\d+h\s*)?(\d+m\s*)?(\d+s)?);";

        public PrintService printService;
        public PrintJobHistory printJob;
        public PrintProgress printProgress;

        public string printName;
        public string estimatedTime;
        public double fileSize;
        public string extractedSettings;
        public string timeElapsed;
        public List<string> files;
        public string file;
        public string fileToPrint = "";
        public bool _processing = false;

        public int hours;
        public int minutes;
        public int seconds;




        public PrintingService()
        {
            printService = new PrintService(ConnectionServiceSerial.printerConnection);
            printJob = new PrintJobHistory(ConnectionServiceSerial.printerConnection);
            printProgress = new PrintProgress();
        }

        public void StartPrint(string fileName)
        {
            printService.StartPrint(fileName);
        }

        public void PausePrint()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPauseSDPrintCommand());
        }

        public void StopPrint()
        {
            printService.AbortCurrentPrint();
        }

        public void ListSDFiles()
        {
            files = printService.ListSDFiles();

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

        public void StartPCPrint(string file, string fileName)
        {
            printService.TransferFileToSD(file, fileName);
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileNameAndSize(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB, 2);
        }

        public void EstimatedPrintTime()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPrintProgressCommand());
            string timeLeft = ConnectionServiceSerial.printerConnection.Read();
            Match match = Regex.Match(timeLeft, PATTERN, RegexOptions.IgnoreCase);
            estimatedTime = match.Groups[1].Value;
        }



        public void Confirm(IDialogService dialogService)
        {
            var parameters = new DialogParameters<Dialog>
            {
                { x => x.ContentText, "Start Print ?" },
                { x => x.ButtonText, "Yes" },
                { x => x.Color, Color.Success }
            };

            IDialogReference dialogReference = dialogService.Show<Dialog>("Confirm", parameters);

            StartPrint(fileToPrint);
            StartTimeOfPrint();
            GetFileNameAndSize(fileToPrint);
        }

        // private void StartClock()
        // {
        //     hours = 0;
        //     minutes = 0;
        //     seconds = 0;

        //     timer = new System.Timers.Timer(1000);
        //     timer.Elapsed += (sender, e) => UpdateClock();
        //     timer.AutoReset = true;
        //     timer.Enabled = true;
        // }

        //private void UpdateClock()
        //{
        //    seconds++;
        //    if (seconds == 60)
        //    {
        //        seconds = 0;
        //        minutes++;
        //        if (minutes == 2)
        //        {
        //            EstimatedPrintTime();
        //        }
        //        if (minutes == 60)
        //        {
        //            minutes = 0;
        //            hours++;
        //        }
        //    }
        //}
    }
}
