using MachineControlHub.Print;
using MachineControlHub.Motion;
using System.Text.RegularExpressions;

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

        public List<string> ListSDFiles() 
        {
            return printService.ListSDFiles();
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
            printService.TransferFileToSD(file,fileName);
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileNameAndSize(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB,2);
        }

        public void EstimatedPrintTime()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPrintProgressCommand());
            Thread.Sleep(500);
            string timeLeft = ConnectionServiceSerial.printerConnection.Read();
            Match match = Regex.Match(timeLeft, PATTERN,RegexOptions.IgnoreCase);
            estimatedTime = match.Groups[1].Value;
        }
    }
}
