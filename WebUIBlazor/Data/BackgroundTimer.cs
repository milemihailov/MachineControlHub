
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace WebUI.Data
{
    public class BackgroundTimer
    {
        public ConnectionServiceSerial ConnectionServiceSerial;


        public readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(10));

        public CancellationTokenSource _cts = new();
        public Stopwatch stopwatch = new Stopwatch();

        public List<string> ConnectionsHistory = new List<string>();
        public PrinterDataServiceTest PrinterDataService;

        public event Action SecondElapsed;
        public event Action FiveSecondElapsed;
        public event Action<string> MessageReceived;
        public event Action BusyStatusChanged;
        public event Action ConnectionStatusChanged;

        public string Message { get; set; }
        public string Notification { get; set; }

        [Parameter]
        public bool IsBusy { get; set; }

        BackgroundTimer()
        {
            ConnectionServiceSerial = new ConnectionServiceSerial();

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

        public void SavePortName()
        {
            if (!ConnectionsHistory.Contains(ConnectionServiceSerial.portName))
            {
                ConnectionsHistory.Add(ConnectionServiceSerial.portName);
            }
        }

        public void StartTimer()
        {
            _ = DoWorkAsync();
        }

        public void StopTimer()
        {
            _cts.Cancel();
        }


        public async Task DoWorkAsync()
        {
            //foreach (var p in PrinterDataServiceTest.Printers)
            //{
            //    Console.WriteLine(p);
            //}

            int i = 0;
            DateTime lastDataReceivedTime = DateTime.Now;

            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    i++;
                    if (i % 100 == 0 && ConnectionServiceSerial.IsConnected)
                    {
                        SecondElapsed?.Invoke();
                    }

                    if (ConnectionServiceSerial != null && ConnectionServiceSerial.HasData())
                    {
                        ConnectionServiceSerial.IsConnected = true;
                        string readData = await ConnectionServiceSerial.printerConnection.ReadAsync();
                        //string data = "";
                        //data += readData;
                        string echoMessage = "";

                        //if (data.ToLower().Contains("echo"))
                        //{
                        //    isBusy = true;
                        //    //BusyStatusChanged?.Invoke();
                        //    await Task.Run(async () =>
                        //    {
                        //        if(readData.ToLower().Contains("echo:busy: processing"))
                        //        {
                        //            //Console.WriteLine("Printer is busy");
                        //            await Task.Delay(1500);
                        //            readData = await ConnectionServiceSerial.printerConnection.ReadAllAsync();
                        //            //Console.WriteLine(echoMessage);
                        //        }
                        //    });
                        //    isBusy = false;
                        //    //BusyStatusChanged?.Invoke();
                        //}
                        Message = $"{readData} \n";

                        if (echoMessage != "")
                        {
                            Message = FilterData(echoMessage);
                        }

                        ParseNotifications(Message);

                        MessageReceived?.Invoke(Message);
                        Console.WriteLine(Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer Cancelled");
            }
        }

        public string FilterData(string data)
        {
            var lines = data.Split('\n');
            var filteredLines = lines.Where(line => !line.Contains("echo"));
            var filteredData = string.Join('\n', filteredLines);
            return filteredData;
        }

        public void StartStopwatch()
        {
            stopwatch.Start();
        }

        public void StopStopwatch()
        {
            stopwatch.Stop();
        }

        public void ResetStopwatch()
        {
            stopwatch.Reset();
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
