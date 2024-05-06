
using System.Text;
using Microsoft.AspNetCore.Components;

namespace WebUI.Data
{
    public class BackgroundTimer
    {


        public readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(10));
        public readonly PeriodicTimer _timer2 = new(TimeSpan.FromMilliseconds(1000));

        public CancellationTokenSource _cts = new();

        public event Action SecondElapsed;
        public event Action<string> MessageReceived;
        public event Action BusyStatusChanged;

        public string message { get; set; }

        [Parameter]
        public bool isBusy { get; set; }
        BackgroundTimer()
        {
            StartTimer();
        }

        private static BackgroundTimer _instance;
        public static BackgroundTimer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BackgroundTimer();
                }
                return _instance;
            }
        }

        public void StartTimer()
        {
            _ = DoWorkAsync();
            //_ = DoWorkEachSecondAsync();
        }

        public void StopTimer()
        {
            _cts.Cancel();
        }

        public async Task DoWorkAsync()
        {
            int i = 0;
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    i++;
                    {
                        if (i % 100 == 0 && Data.ConnectionServiceSerial.printerConnection.IsConnected)
                        {
                            SecondElapsed?.Invoke();
                            //Console.WriteLine(DateTime.Now);
                        }
                    }

                    if (Data.ConnectionServiceSerial.printerConnection != null && Data.ConnectionServiceSerial.printerConnection.HasData())
                    {
                        string readData = Data.ConnectionServiceSerial.printerConnection.Read();
                        string data = "";
                        data += readData;
                        string echoMessage = "";

                        if (data.ToLower().Contains("echo"))
                        {
                            isBusy = true;
                            BusyStatusChanged?.Invoke();
                            await Task.Run(async () =>
                            {
                                while (readData.ToLower().Contains("echo:busy: processing"))
                                {
                                    Console.WriteLine("Printer is busy");
                                    await Task.Delay(1500);
                                    echoMessage += readData = ConnectionServiceSerial.printerConnection.ReadAll();
                                }
                            });
                            isBusy = false;
                            BusyStatusChanged?.Invoke();
                        }
                        message = $"{readData} \n";

                        if (echoMessage != "")
                        {
                            message = FilterData(echoMessage);
                        }


                        //Console.WriteLine(message);
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer Cancelled");
            }
        }

        public async Task DoWorkEachSecondAsync()
        {
            int i = 0;
            while (await _timer2.WaitForNextTickAsync(_cts.Token))
            {
                i++;
                if (Data.ConnectionServiceSerial.printerConnection.IsConnected)
                {
                    SecondElapsed?.Invoke();
                    Console.WriteLine(DateTime.Now);
                }
            }
        }


        public string FilterData(string data)
        {
            var lines = data.Split('\n');
            var filteredLines = lines.Where(line => !line.Contains("echo"));
            var filteredData = string.Join('\n', filteredLines);
            return filteredData;
        }

    }

}
