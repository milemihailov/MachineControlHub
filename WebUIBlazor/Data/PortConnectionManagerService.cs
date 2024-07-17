using System.Runtime.CompilerServices;
using System.Threading;

namespace WebUI.Data
{
    public class PortConnectionManagerService
    {

        public SerialDataProcessorService connection;

        public Dictionary<string, SerialDataProcessorService> connections = new();

        private CancellationTokenSource _cancellationTokenSource;

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
        public bool IsBusy => connection.IsBusy;
        public bool IsConnected => connection.IsConnected;

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

        public async void Output(string input, SerialDataProcessorService source)
        {
            if (SelectedPrinter == source.ConnectionServiceSerial.portName)
            {
                if (input.Contains("echo:busy: processing"))
                {
                    connections[SelectedPrinter].IsBusy = true;
                    // Cancel the previous token if it exists
                    _cancellationTokenSource?.Cancel();

                    // Create a new CancellationTokenSource
                    _cancellationTokenSource = new CancellationTokenSource();

                    // Start a new task to reset IsBusy after 5 seconds if no new messages are received
                    var token = _cancellationTokenSource.Token;
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(2000, token);
                            connections[SelectedPrinter].IsBusy = false;
                        }
                        catch (TaskCanceledException)
                        {
                            // Task was canceled, do nothing
                        }
                    }, token);
                }
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