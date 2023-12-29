using MachineControlHub.Print;
using MachineControlHub.Motion;

namespace WebUI.Data
{
    public class PrintingService
    {
        public PrintService printService;
        public PrintJob printJob;

        public PrintingService()
        {
            printService = new PrintService(ConnectionServiceSerial.printerConnection);
            printJob = new PrintJob(ConnectionServiceSerial.printerConnection);
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
    }
}
