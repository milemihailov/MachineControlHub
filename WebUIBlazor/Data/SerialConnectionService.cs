using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class SerialConnectionService
    {

        readonly SerialConnection serialConnection = new();

        public string portName = "";
        public int baudRate;

        
        public void Connect()
        {
            serialConnection.PortName = portName;
            serialConnection.BaudRate = baudRate;
            serialConnection.Connect();
            
        }

        public void Disconnect()
        {
            serialConnection.Disconnect();
        }

        public void Write(string value)
        {
            serialConnection.Write(value);
        }

        public List<string> AvailableConnections() 
        {
            return serialConnection.AvailableConnections();
        }
    }
}
