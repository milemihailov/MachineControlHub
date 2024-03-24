using MachineControlHub;
using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Print;
using MachineControlHub.Temps;
using static MachineControlHub.Bed.PrinterBed;

namespace WebUI.Data
{
    public class PrinterDataServiceTest
    {
        public readonly ConnectionServiceSerial serialConnection;
        public bool IsConnected { get; set; }
        public string selectedShape;
        public List<PreheatingProfiles> profiles = new List<PreheatingProfiles>();

        public Printer Printer = new();

        public PrinterDataServiceTest()
        {
            Printer.HotendTemperatures = new HotendTemps(Printer.PrinterConnection);
            Printer.BedTemperatures = new BedTemps(Printer.PrinterConnection);
            Printer.ChamberTemperatures = new ChamberTemps(Printer.PrinterConnection);
            Printer.Bed = new PrinterBed();
            Printer.Head = new PrinterHead();
            Printer.Camera = new Camera();
            Printer.FilamentProperties = new FilamentProperties();
            Printer.PreheatingProfiles = new PreheatingProfiles();
            Printer.MotionSettings = new MotionSettingsData();
            Printer.PrintHistory = new PrintHistory();
            Printer.CurrentPrintJob = new CurrentPrintJob(Printer.PrinterConnection);
            Printer.TouchScreen = new TouchScreen();
        }

        public void SelectBedShape(string shape)
        {
            switch (shape)
            {
                case "Rectangular":
                    Printer.Bed.ShapeOfBed = BedShapes.Rectangular;
                    break;
                case "Circular":
                    Printer.Bed.ShapeOfBed = BedShapes.Circular;
                    break;
                case "Custom":
                    Printer.Bed.ShapeOfBed = BedShapes.Custom;
                    break;
                default:
                    break;
            }
        }


        public void HandleValidSubmit()
        {
            profiles.Add(Printer.PreheatingProfiles);
            Printer.PreheatingProfiles = new PreheatingProfiles();
        }
    }
}
