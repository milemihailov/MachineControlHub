using System.Runtime.CompilerServices;

namespace WebUI.Data
{
    public class PortConnectionManagerService
    {

        public SerialDataProcessorService connection;

        public Dictionary<string, SerialDataProcessorService> connections = new();

        private string _selectedPrinter = "";
        public string SelectedPrinter
        {
            get => _selectedPrinter;
            set
            {
                if (_selectedPrinter != value)
                {
                    _selectedPrinter = value;
                    OnSelectedPrinterChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedPrinterChanged;

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
            if(SelectedPrinter == source.ConnectionServiceSerial.portName)
            {
                //Console.WriteLine($"{source.ConnectionServiceSerial.portName}{input}");
                //Console.WriteLine($"{source.ConnectionServiceSerial.portName}{source.Notification}");
            }
        }

        public void SelectPrinter(string comport)
        {
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

        protected virtual void OnSelectedPrinterChanged(EventArgs e)
        {
            SelectedPrinterChanged?.Invoke(this, e);
        }
    }
}