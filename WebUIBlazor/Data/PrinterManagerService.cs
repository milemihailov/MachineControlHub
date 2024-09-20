using System.Collections.Concurrent;
using System.Text.RegularExpressions;
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
        private ConcurrentDictionary<string, string> PortData { get; set; } = new();

        public Dictionary<string, Printer> Printers { get; set; } = new();

        public event Action<string> InputReceived;
        public event EventHandler ActivePrinterChanged;
        public event Action PrinterBusy;
        public string Notification { get; set; }
        public string UnknownCommandMessage { get; set; }

        public PrinterManagerService()
        {
            BackgroundTimer.TenMilisecondsElapsed += ReadFromAllPortsAsync;
            ActivePrinter = new()
            {
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
                SerialConnection = new(),
                Position = new()
            };

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
                    Position = new(),
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

        /// <summary>
        /// Asynchronously reads data from all connected printers' ports.
        /// </summary>
        /// <remarks>
        /// This method cancels any existing read operations, creates a new cancellation token,
        /// and starts reading from all printers' ports concurrently. If any task is canceled,
        /// it catches the exception and does nothing.
        /// </remarks>
        public async void ReadFromAllPortsAsync()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
            }

            CancellationTokenSource = new CancellationTokenSource();
            var tasks = Printers.Values.Select(printer => ReadFromPort(printer, CancellationTokenSource.Token));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Task was canceled, do nothing
            }
        }

        /// <summary>
        /// Asynchronously reads data from a specified printer's port until the cancellation token is triggered.
        /// </summary>
        /// <param name="printer">The printer object from which to read data.</param>
        /// <param name="token">The cancellation token used to cancel the read operation.</param>
        /// <remarks>
        /// This method continuously reads data from the printer's serial connection. If the data contains
        /// "echo:busy: processing", it sets the printer's busy state and resets it after a delay. The method
        /// also filters the data, updates the port data, parses notifications, and updates the bed level data.
        /// </remarks>
        private async Task ReadFromPort(Printer printer, CancellationToken token)
        {
            if (!printer.IsBusy)
            {
                while (!token.IsCancellationRequested)
                {
                    if (printer.SerialConnection != null && printer.SerialConnection.HasData())
                    {
                        printer.SerialConnection.IsConnected = true;
                        string readData = await printer.SerialConnection.ReadAsync();

                        if (readData.Contains("echo:busy: processing"))
                        {
                            printer.IsBusy = true;
                            printer.CancellationTokenSource?.Cancel();
                            printer.CancellationTokenSource = new CancellationTokenSource();
                            var resetToken = printer.CancellationTokenSource.Token;
                            PrinterBusy?.Invoke();
                            await Task.Run(async () =>
                            {
                                try
                                {
                                    await Task.Delay(2000, resetToken);
                                    printer.IsBusy = false;
                                }
                                catch (TaskCanceledException)
                                {
                                    // Task was canceled, do nothing
                                }
                            }, resetToken);
                        }

                        string input = $"{readData} \n";
                        PortData[printer.SerialConnection.PortName] = input;

                        if (printer == ActivePrinter)
                        {
                            ParseNotifications(input);
                            ParseUnknownCommandMessage(input);
                            InputReceived?.Invoke(input);
                            Console.WriteLine($"{printer.SerialConnection.PortName} : {readData}");
                        }

                        printer.BedLevelData.OnBedLevelUpdate(input);
                        await printer.CurrentPrintJob.EstimateTimeRemainingAsync();
                    }
                    await Task.Delay(600, token);
                }
            }
        }


        /// <summary>
        /// Parses notification and prompt actions from the provided data string.
        /// </summary>
        /// <param name="data">The input data string to be parsed for notifications and prompts.</param>
        /// <remarks>
        /// This method uses regular expressions to identify and extract notification and prompt actions
        /// from the input data. If a notification is found, it updates the <see cref="Notification"/> property.
        /// </remarks>
        public void ParseNotifications(string data)
        {
            string patternNotification = @"//action:notification\s*(.*)";

            if (data.Contains("//action:notification"))
            {
                Match match = Regex.Match(data, patternNotification);
                if (match.Success)
                {
                    string result = match.Groups[1].Value;
                    Notification = result;
                }
            }
        }

        public void ParseUnknownCommandMessage(string input)
        {
            if (input.Contains("Unknown command:"))
            {
                UnknownCommandMessage = input;
            }
        }
    }
}