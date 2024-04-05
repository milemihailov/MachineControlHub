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
        public Printer Printer = new Printer();
        public readonly ConnectionServiceSerial serialConnection;
        private readonly ISnackbar _snackbar;

        public bool IsConnected { get; set; }

        public string selectedShape;

        public List<PreheatingProfiles> preheatingProfiles = new List<PreheatingProfiles>();
        public List<Printer> Printers = new List<Printer>();
        public List<PrinterHead> Extruders { get; set;}
        public List<PrinterHead> ExtruderSettings = new List<PrinterHead>();
        public PrinterBed Bed { get; set;}


        public Printer SelectedPrinter;

        public int XMaxFeedrate { get; set; }
        public int YMaxFeedrate { get; set; }
        public int ZMaxFeedrate { get; set; }
        public int EMaxFeedrate { get; set; }

        public string PrinterName { get; set; }
        public int NumOfExtruders = 1;
        public string Model { get; set; }
        public string PrinterFirmwareVersion { get; set; }
        public bool HasAutoBedLevel { get; set; }
        public bool HasChamber { get; set; }
        public bool HasHeatedBed = true;
        public bool HasFilamentRunoutSensor { get; set; }
        public int ZMaxBuildVolume { get; set; }
        public bool HasCamera { get; set; }
        public bool HasPartCoolingFan { get; set; }
        public bool HasProbe { get; set; }
        public double NozzleDiameter { get;  set; }
        public string NozzleMaterial { get;  set; }

        public PrinterDataServiceTest(ISnackbar snackbar)
        {
            _snackbar = snackbar;
            Printer.HotendTemperatures = new HotendTemps(Printer.PrinterConnection);
            Printer.BedTemperatures = new BedTemps(Printer.PrinterConnection);
            Printer.ChamberTemperatures = new ChamberTemps(Printer.PrinterConnection);
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
            Printer.Model = Model;
            Printer.NumberOfExtruders = NumOfExtruders;
            Printer.PrinterFirmwareVersion = PrinterFirmwareVersion;
            Printer.HasAutoBedLevel = HasAutoBedLevel;
            Printer.HasChamber = HasChamber;
            Printer.HasHeatedBed = HasHeatedBed;
            Printer.HasFilamentRunoutSensor = HasFilamentRunoutSensor;
            Printer.ZMaxBuildVolume = ZMaxBuildVolume;
            Extruders = new List<PrinterHead>();
            SelectedPrinter = new Printer();
            Printer.Bed = new PrinterBed();
        }

        public void ChoosePrinter(Printer printer)
        {
            SelectedPrinter = printer;
        }

        public void UpdateExtruders(int newValue)
        {
            while (ExtruderSettings.Count < newValue)
            {
                ExtruderSettings.Add(new PrinterHead());
            }
            while (ExtruderSettings.Count > newValue)
            {
                ExtruderSettings.RemoveAt(ExtruderSettings.Count - 1);
            }
        }

        public void CreatePrinterProfile()
        {
            var newPrinter = new Printer();
            newPrinter.Extruders = new List<PrinterHead>();
            newPrinter.Bed = new PrinterBed();
            newPrinter.Name = PrinterName;
            newPrinter.Id = $"#{Printers.Count}";
            newPrinter.Model = Model;
            newPrinter.NumberOfExtruders = NumOfExtruders;
            newPrinter.PrinterFirmwareVersion = PrinterFirmwareVersion;
            newPrinter.HasAutoBedLevel = HasAutoBedLevel;
            newPrinter.HasChamber = HasChamber;
            newPrinter.HasHeatedBed = HasHeatedBed;
            newPrinter.HasFilamentRunoutSensor = HasFilamentRunoutSensor;
            newPrinter.ZMaxBuildVolume = ZMaxBuildVolume;
            newPrinter.Bed.XSize = Printer.Bed.XSize;
            newPrinter.Bed.YSize = Printer.Bed.YSize;
            newPrinter.Bed.Diameter = Printer.Bed.Diameter;


            for (int i = 0; i < ExtruderSettings.Count; i++)
            {
                var newExtruder = new PrinterHead();
                
                newExtruder.HasCoolingFan = ExtruderSettings[i].HasCoolingFan;
                newExtruder.ProbePresent = ExtruderSettings[i].ProbePresent;
                newExtruder.NozzleDiameter = ExtruderSettings[i].NozzleDiameter;
                newExtruder.NozzleMaterial = ExtruderSettings[i].NozzleMaterial;

                newPrinter.Extruders.Add(newExtruder);
            }

            Printers.Add(newPrinter);

            SelectedPrinter = newPrinter;
        }

        public void CreatePreheatProfile()
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

            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildMaxFeedrateCommand(Printer.MotionSettings));
        }

        public void SaveToEEPROM()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildSaveToEEPROMCommand());
        }
    }
}
