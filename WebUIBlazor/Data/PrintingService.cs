using MachineControlHub.Print;
using MachineControlHub.Motion;

namespace WebUI.Data
{
    public class PrintingService
    {
        public PrintService printService;
        public PrintJobHistory printJob;
        public PrintProgress printProgress;

        public string printName;
        public DateTime startTimeOfPrint;
        public double fileSize;

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

        public void StartSDPrint(string gcode, string fileName)
        {
            printService.TransferFileToSD(gcode, fileName);
        }

        public void StartTimeOfPrint()
        {
            printJob.ParseStartTimeOfPrint();
            startTimeOfPrint = (DateTime)printJob.StartTimeOfPrint;
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileName(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB,2);
        }
    }
}
