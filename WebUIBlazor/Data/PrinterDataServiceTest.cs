using MachineControlHub;
using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Print;
using MachineControlHub.Temps;
using MudBlazor;
using static MachineControlHub.Bed.PrinterBed;

namespace WebUI.Data
{
    public class PrinterDataServiceTest
    {
        public readonly ConnectionServiceSerial serialConnection;
        private readonly ISnackbar _snackbar;

        public bool IsConnected { get; set; }
        public string selectedShape;
        public List<PreheatingProfiles> preheatingProfiles = new List<PreheatingProfiles>();
        public int XMaxFeedrate { get; set; }
        public int YMaxFeedrate { get; set; }
        public int ZMaxFeedrate { get; set; }
        public int EMaxFeedrate { get; set; }


        public Printer Printer = new();

        public PrinterDataServiceTest(ISnackbar snackbar)
        {
            _snackbar = snackbar;
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

            Printer.MotionSettings.XMaxFeedrate = XMaxFeedrate;
            Printer.MotionSettings.YMaxFeedrate = YMaxFeedrate;
            Printer.MotionSettings.ZMaxFeedrate = ZMaxFeedrate;
            Printer.MotionSettings.EMaxFeedrate = EMaxFeedrate;
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
            preheatingProfiles.Add(Printer.PreheatingProfiles);
            Printer.PreheatingProfiles = new PreheatingProfiles();
            _snackbar.Add("Preset Added To Print Section", Severity.Success);
        }

        public void StartPreheating(PreheatingProfiles profile)
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildSetHotendTempCommand(profile.HotendTemp));
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildSetBedTempCommand(profile.BedTemp));
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFanSpeedCommand(profile.FanSpeed));
            _snackbar.Add("Preheating Started", Severity.Success);
        }

        public void DeletePreheatingProfile(PreheatingProfiles profile)
        {
            preheatingProfiles.Remove(profile);
            _snackbar.Add("Preset Removed", Severity.Error);
        }

        public void SendPreheatingProfiles()
        {
            foreach (var profile in preheatingProfiles)
            {
                ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildMaterialPresetCommand(profile));
                _snackbar.Add("Preset Added To Printer", Severity.Success);
            }
        }

        public void SetMaxFeedrates()
        {
            Printer.MotionSettings.XMaxFeedrate = XMaxFeedrate;
            Printer.MotionSettings.YMaxFeedrate = YMaxFeedrate;
            Printer.MotionSettings.ZMaxFeedrate = ZMaxFeedrate;
            Printer.MotionSettings.EMaxFeedrate = EMaxFeedrate;

            ConnectionServiceSerial.printerConnection.Write( CommandMethods.BuildMaxFeedrateCommand(Printer.MotionSettings));
        }

        public void SaveToEEPROM()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildSaveToEEPROMCommand());
        }
    }
}
