using System.Text.RegularExpressions;

namespace WebUI.Data
{
    public class SerialDataProcessor
    {
        public SerialConnectionService ConnectionServiceSerial;

        public BackgroundTimer BackgroundTimer;

        public event Action<string> InputReceived;
        public event Action ConnectionStatusChanged;

        public bool IsConnected { get; set; }

        private string Input { get; set; }
        private string Notification { get; set; }

        public SerialDataProcessor(string portName = null, string baudRate = null)
        {
            ConnectionServiceSerial = new SerialConnectionService();
            if (portName != null)
            {
                ConnectionServiceSerial.portName = portName;
                ConnectionServiceSerial.baudRate = int.Parse(baudRate);
            }
            BackgroundTimer = new BackgroundTimer();
            BackgroundTimer.SecondElapsed += ReadFromPort;
        }

        public async void ReadFromPort()
        {
            if(ConnectionServiceSerial != null && ConnectionServiceSerial.HasData())
            {
                ConnectionStatusChanged?.Invoke();
                ConnectionServiceSerial.IsConnected = true;
                string readData = await ConnectionServiceSerial.printerConnection.ReadAsync();
                string echoMessage = "";

                Input = $"{readData} \n";

                if (echoMessage != "")
                {
                    Input = FilterData(echoMessage);
                }

                ParseNotifications(Input);

                InputReceived?.Invoke(Input);

                Console.WriteLine(readData);
            }
        }

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

        public override string ToString()
        {
            return $"Port: {ConnectionServiceSerial.portName}";
        }

    }
}
