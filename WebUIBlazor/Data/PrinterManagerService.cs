using MachineControlHub;
using MachineControlHub.Print;
using MachineControlHub.PrinterConnection;

namespace WebUI.Data
{
    public class PrinterManagerService
    {
        public Printer ActivePrinter { get; set; } = new();
        public SerialConnection PrinterSerialConnection { get; set; }
        public BackgroundTimer BackgroundTimer { get; set; } = new();


        public Dictionary<string, Printer> Printers { get; set; } = new();

        public event Action<string> InputReceived;

        private string Input { get; set; }

        public PrinterManagerService()
        {
            BackgroundTimer.TenMilisecondsElapsed += ReadFromPort;
            ActivePrinter = new();
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
                    CurrentPrintJob = new CurrentPrintJob(),
                    PrintHistory = new(),
                    PrintService = new PrintService(PrinterSerialConnection),
                    HotendTemperatures = new(PrinterSerialConnection),
                    BedTemperatures = new(PrinterSerialConnection),
                    ChamberTemperatures = new(PrinterSerialConnection),
                    MotionSettings = new(),
                    StepperDrivers = new(),
                    Bed = new(),
                    Head = new(),
                });
            }

            SelectPrinter(comport);
        }

        public void SelectPrinter(string comPort)
        {
            if (Printers.ContainsKey(comPort))
            {
                ActivePrinter = Printers[comPort];
            }
        }

        public async void ReadFromPort()
        {
            if (ActivePrinter.SerialConnection != null && ActivePrinter.SerialConnection.HasData())
            {
                ActivePrinter.SerialConnection.IsConnected = true;
                string readData = await ActivePrinter.SerialConnection.ReadAsync();
                string echoMessage = "";

                Input = $"{readData} \n";

                if (echoMessage != "")
                {
                    Input = FilterData(echoMessage);
                }

                //ParseNotifications(Input);

                InputReceived?.Invoke(Input);
                Console.WriteLine($"{ActivePrinter.SerialConnection.PortName} : {readData}");
            }
        }

        public string FilterData(string data)
        {
            var lines = data.Split('\n');
            var filteredLines = lines.Where(line => !line.Contains("echo"));
            var filteredData = string.Join('\n', filteredLines);
            return filteredData;
        }

        public void TestButton()
        {
            Console.WriteLine(ActivePrinter.MotionSettings.XStepsPerUnit);
            Console.WriteLine(ActivePrinter.Bed.XSize);
            Console.WriteLine(ActivePrinter.Bed.YSize);
        }
    }
}