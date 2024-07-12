using System.Text.RegularExpressions;
using WebUI.Pages;

namespace WebUI.Data
{
    public class SerialDataProcessorService
    {
        public SerialConnectionService ConnectionServiceSerial;

        public BackgroundTimer BackgroundTimer;

        public event Action<string, SerialDataProcessorService> InputReceived;

        public event Action ConnectionStatusChanged;
        public string SelectedPort { get; set; }
        public bool IsConnected { get; set; }
        private string Input { get; set; }
        public string Notification { get; set; }

        public SerialDataProcessorService(string portName = null, int? baudRate = null)
        {
            ConnectionServiceSerial = new SerialConnectionService();
            if (portName != null && baudRate.HasValue)
            {
                ConnectionServiceSerial.portName = portName;
                ConnectionServiceSerial.baudRate = baudRate.Value;
            }
            BackgroundTimer = new BackgroundTimer();
            BackgroundTimer.TenMilisecondsElapsed += ReadFromPort;
        }

        public async void ReadFromPort()
        {
            if (ConnectionServiceSerial != null && ConnectionServiceSerial.HasData())
            {
                ConnectionServiceSerial.IsConnected = true;
                string readData = await ConnectionServiceSerial.printerConnection.ReadAsync();
                string echoMessage = "";

                Input = $"{readData} \n";

                if (echoMessage != "")
                {
                    Input = FilterData(echoMessage);
                }

                ParseNotifications(Input);

                InputReceived?.Invoke(Input, this);
                Console.WriteLine($"{ConnectionServiceSerial.portName} : {readData}");
                //Console.WriteLine($"This is selected port: {SelectedPort}");
            }
        }

        //maybe :) public List<string> Outupt { get; set; } = new();

        public string FilterData(string data)
        {
            var lines = data.Split('\n');
            var filteredLines = lines.Where(line => !line.Contains("echo"));
            var filteredData = string.Join('\n', filteredLines);
            return filteredData;
        }

        public void ParseNotifications(string data)
        {
            string patternNotification = @"//action:notification\s*(.*)";
            string patternPrompt = @"//action:prompt\s*(.*)";

            if (data.Contains("//action:notification"))
            {
                Match match = Regex.Match(data, patternNotification);
                if (match.Success)
                {
                    string result = match.Groups[1].Value;
                    Notification = result;
                    Console.WriteLine(result);
                }
            }


            if (data.Contains("//action:prompt"))
            {
                Match match = Regex.Match(data, patternPrompt);
                if (match.Success)
                {
                    string result = match.Groups[1].Value;
                    //Notification = result;
                    Console.WriteLine(result);
                }
            }
        }
    }
}
