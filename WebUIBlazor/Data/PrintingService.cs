using MachineControlHub.Print;
using MachineControlHub.Motion;
using Microsoft.AspNetCore.Components.Forms;

namespace WebUI.Data
{
    public class PrintingService
    {
        public PrintService printService;
        public PrintJobHistory printJob;
        public PrintProgress printProgress;

        public string printName;
        public double fileSize;
        public string formatedStartTime;

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
            formatedStartTime = printJob.FormattedStartTime;
            
        }

        public void GetFileNameAndSize(string input)
        {
            printProgress.ParseFileNameAndSize(input);
            printName = printProgress.PrintingFileName;
            fileSize = Math.Round(printProgress.FileSizeInMB,2);
        }
    }
}
