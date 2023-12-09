using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class SerialConnectionService
    {

        public IPrinterConnection printerConnection;

        public void Write(string value)
        {
            printerConnection.Write(value);
        }
    }
}