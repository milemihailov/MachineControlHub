using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using Microsoft.AspNetCore.Components;

namespace WebUI.Data
{
    public class ConnectionServiceSerial
    {

        public static SerialConnection printerConnection;

        public bool inititalized = false;


        public ConnectionServiceSerial()
        {
            printerConnection = new SerialConnection();
        }

        public void Connect()
        {
            printerConnection.Connect();
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
            inititalized = false;
        }

        public List<string> GetPorts()
        {
            return printerConnection.AvailableConnections();
        }
    }
}