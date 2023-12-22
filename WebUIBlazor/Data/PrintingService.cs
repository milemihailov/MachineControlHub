using MachineControlHub.Print;

namespace WebUI.Data
{
    public class PrintingService
    {
        public PrintService printService;
        public PrintJob printJob;

        public PrintingService()
        {
            printService = new PrintService(Data.ConnectionServiceSerial.printerConnection);
            printJob = new PrintJob(Data.ConnectionServiceSerial.printerConnection);
        }

        public void SelectFile(string fileName)
        {
            printService.SelectPrint(fileName);
        }
    }
}
