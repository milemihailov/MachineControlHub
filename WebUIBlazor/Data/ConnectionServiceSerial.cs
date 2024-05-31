using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class ConnectionServiceSerial
    {
        public IPrinterConnection printerConnection;
        public string portName = "";
        public int baudRate = 115200;

        public bool IsConnected { get; set; }

        public ConnectionServiceSerial()
        {
            printerConnection = new SerialConnection();
            var ports = GetPorts();
            if (ports.Any())
            {
                portName = ports.First();
            }
        }

        public void Connect()
        {
            printerConnection.Connect();
        }

        public void Close()
        {
            printerConnection.Close();
        }

        public void Initialize(string connectionString)
        {
            printerConnection.Initialize(connectionString);
        }

        public void Write(string value)
        {
            printerConnection.Write(value);
        }

        public string Read()
        {
            return printerConnection.Read();
        }

        public void Disconnect()
        {
            printerConnection.Disconnect();
            printerConnection.IsConnected = false;
        }

        public List<string> GetPorts()
        {
            return printerConnection.AvailableConnections();
        }

        public bool HasData()
        {
            return printerConnection.HasData();
        }
    }
}