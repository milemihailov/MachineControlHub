using System.Text.RegularExpressions;
using System.Threading;
using MachineControlHub;
using MachineControlHub.Print;
using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class PrinterManagerService
    {
        public Printer ActivePrinter { get; set; } = new();
        public SerialConnection PrinterSerialConnection { get; set; } = new();
        public BackgroundTimer BackgroundTimer { get; set; } = new();
        private CancellationTokenSource CancellationTokenSource { get; set; }


        public Dictionary<string, Printer> Printers { get; set; } = new();

        public event Action<string> InputReceived;

        public event EventHandler ActivePrinterChanged;

        private string Input { get; set; }

        public string Notification { get; set; }

        public PrinterManagerService()
        {
            BackgroundTimer.TenMilisecondsElapsed += ReadFromPort;
        }

        public void AddPrinter(string comport, int baudrate, string name = null)
        {
            if (!Printers.ContainsKey(comport))
            {
                PrinterSerialConnection = new()
                {
                    BaudRate = baudrate,
                    PortName = comport
                };
                name ??= comport;

                PrinterSerialConnection.Initialize($"{comport},{baudrate}");
                PrinterSerialConnection.Connect();
                PrinterSerialConnection.IsConnected = true;
                Printers.Add(comport, new Printer()
                {
                    Name = name,
                    SerialConnection = PrinterSerialConnection,
                    CurrentPrintJob = new CurrentPrintJob(PrinterSerialConnection),
                    PrintHistory = new(),
                    PrintService = new PrintService(PrinterSerialConnection),
                    HotendTemperatures = new(PrinterSerialConnection),
                    BedTemperatures = new(PrinterSerialConnection),
                    ChamberTemperatures = new(PrinterSerialConnection),
                    PreheatingProfiles = new(),
                    MotionSettings = new(),
                    StepperDrivers = new(),
                    BedLevelData = new(),
                    Bed = new(),
                    Head = new(),
                });
            }
        }

        public void SelectPrinter(string comPort)
        {
            if (Printers.ContainsKey(comPort))
            {
                ActivePrinter = Printers[comPort];
                Notification = null;
                ActivePrinterChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void ReadFromPort()
        {
            if (ActivePrinter.SerialConnection != null && ActivePrinter.SerialConnection.HasData())
            {
                ActivePrinter.SerialConnection.IsConnected = true;
                string readData = await ActivePrinter.SerialConnection.ReadAsync();
                string echoMessage = "";

                if (readData.Contains("echo:busy: processing"))
                {
                    ActivePrinter.IsBusy = true;
                    // Cancel the previous token if it exists
                    CancellationTokenSource?.Cancel();

                    // Create a new CancellationTokenSource
                    CancellationTokenSource = new CancellationTokenSource();

                    // Start a new task to reset IsBusy after 5 seconds if no new messages are received
                    var token = CancellationTokenSource.Token;
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(2000, token);
                            ActivePrinter.IsBusy = false;
                        }
                        catch (TaskCanceledException)
                        {
                            // Task was canceled, do nothing
                        }
                    }, token);
                }

                Input = $"{readData} \n";

                if (echoMessage != "")
                {
                    Input = FilterData(echoMessage);
                }


                ParseNotifications(Input);

                InputReceived?.Invoke(Input);
                //Console.WriteLine($"{ActivePrinter.SerialConnection.PortName} : {readData}");

                ActivePrinter.BedLevelData.OnBedLevelUpdate(Input);
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
    }
}