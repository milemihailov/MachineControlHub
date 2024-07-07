using System.Runtime.CompilerServices;

namespace WebUI.Data
{
    public class PortConnectionManagerService
    {

        public SerialDataProcessorService connection;

        public Dictionary<string, SerialDataProcessorService> connections = new();

        public string SelectPrinterString = "";

        public string portName = "";
        public int baudRate = 115200;

        public PortConnectionManagerService()
        {
            connection = new SerialDataProcessorService();
            //connection.InputReceived += Output;
        }

        public void CreateConnection(string comport, int baudrate)
        {
            if (!connections.ContainsKey(comport))
            {
                connections.Add(comport, new SerialDataProcessorService(comport,baudrate));
                connections[comport].InputReceived += Output;
            }
            foreach (var item in connections)
            {
                Console.WriteLine(item);
            }
        }

        public void Output(string input, SerialDataProcessorService source)
        {
            if(SelectPrinterString == source.ConnectionServiceSerial.portName)
            {
                //Console.WriteLine($"{source.ConnectionServiceSerial.portName}{input}");
                //Console.WriteLine($"{source.ConnectionServiceSerial.portName}{source.Notification}");
            }
        }

        public void SelectPrinter(string comport)
        {
            Console.WriteLine("called");
            if (connections.ContainsKey(comport))
            {
                connection = connections[comport];
                connection.SelectedPort = comport;
                Console.WriteLine("Selected: " + connection.SelectedPort);
            }
            else
            {
                Console.WriteLine($"No printer found with COM port: {comport}");
            }
        }

        public void SendTestCommand()
        {
            connections[SelectPrinterString].ConnectionServiceSerial.Write("M503");
        }

        public void SendTestCommand1()
        {
            connections[SelectPrinterString].ConnectionServiceSerial.Write("M27 S1");
        }
    }
}