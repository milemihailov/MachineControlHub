using System.Runtime.CompilerServices;

namespace WebUI.Data
{
    public class PortConnectionManager
    {

        public SerialDataProcessor connection;

        public Dictionary<string, SerialDataProcessor> connections = new();

        public string SelectedPrinter { get; set; }

        public PortConnectionManager()
        {
            connection = new SerialDataProcessor();
            //SelectedPrinter = connections[connection.ConnectionServiceSerial.portName];
        }

        public void CreateConnection(string comport, string baudrate)
        {
            if (!connections.ContainsKey(comport))
            {
                connections.Add(comport, new SerialDataProcessor(comport,baudrate));
            }
            foreach (var item in connections)
            {
                Console.WriteLine(item);
            }
        }

        public void SelectPrinter(string comport)
        {
            if (connections.ContainsKey(comport))
            {
                connection = connections[comport];
            }
            else
            {
                Console.WriteLine($"No printer found with COM port: {comport}");
            }
        }
    }
}