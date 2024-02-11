using MachineControlHub.Print;
using MachineControlHub.Motion;

namespace WebUI.Data
{
    public class PrintingService
    {
        public const int MAX_FILE_SIZE = (1024 * 1024 * 30);

        public PrintService printService;
        public PrintJobHistory printJob;
        public PrintProgress printProgress;

        public string printName;
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
    }
}
