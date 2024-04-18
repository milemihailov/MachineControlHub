using System.Text.Json;
using MachineControlHub;
using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Print;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class PrinterDataServiceTest
    {
        public const string PRINTER_DATA_PATH = "printers.json";
        public const string PREHEATING_PROFILES_PATH = "preheatingProfiles.json";
        public const string SELECTED_PRINTER_SETTINGS_PATH = "selectedPrinter.json";

        public Printer Printer;
        public readonly ConnectionServiceSerial serialConnection;
        private readonly ISnackbar _snackbar;

        public bool IsConnected { get; set; }

        public string selectedShape;

        public List<PreheatingProfiles> preheatingProfiles = new List<PreheatingProfiles>();
        public List<Printer> Printers { get; set; }
        public List<PrinterHead> Extruders { get; set; }
        public List<PrinterHead> ExtruderSettings = new List<PrinterHead>();
        public PrinterBed Bed { get; set; }


        public Printer SelectedPrinter;
        public bool HasCamera { get; set; }

        public PrinterDataServiceTest(ISnackbar snackbar)
        {
            Printer = new Printer();
            _snackbar = snackbar;
            Printer.HotendTemperatures = new HotendTemps(Printer.PrinterConnection);
            Printer.BedTemperatures = new BedTemps(Printer.PrinterConnection);
            Printer.ChamberTemperatures = new ChamberTemps(Printer.PrinterConnection);
            Printer.Camera = new Camera();
            Printer.PreheatingProfiles = new PreheatingProfiles();
            Printer.MotionSettings = new MotionSettingsData();
            Printer.PrintHistory = new PrintHistory();
            Printer.CurrentPrintJob = new CurrentPrintJob(Printer.PrinterConnection);
            Printer.TouchScreen = new TouchScreen();
            Extruders = new List<PrinterHead>();
            Printer.Bed = new PrinterBed();
            Printers = new List<Printer>();
            SelectedPrinter = new Printer();
        }

        public void ChoosePrinter(Printer printer)
        {
            SelectedPrinter = printer;
            SavePrinterData(SELECTED_PRINTER_SETTINGS_PATH, SelectedPrinter);
        }

        public void RemovePrinter(Printer printer)
        {
            Printers.Remove(printer);
            SavePrinterData(PRINTER_DATA_PATH, Printers);
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

        public string GenerateUniquePrinterId()
        {
            Random random = new Random();
            string newId;
            do
            {
                newId = $"#{random.Next(1, 10000)}";
            }
            while (Printers.Any(p => p.Id == newId));

            return newId;
        }

        public void CreatePrinterProfile()
        {
            var newPrinter = new Printer();
            newPrinter.Extruders = new List<PrinterHead>();
            newPrinter.Bed = new PrinterBed();
            newPrinter.Name = Printer.Name;
            newPrinter.Id = GenerateUniquePrinterId();
            newPrinter.Model = Printer.Model;
            newPrinter.NumberOfExtruders = Printer.NumberOfExtruders;
            newPrinter.PrinterFirmwareVersion = Printer.PrinterFirmwareVersion;
            newPrinter.HasAutoBedLevel = Printer.HasAutoBedLevel;
            newPrinter.HasChamber = Printer.HasChamber;
            newPrinter.HasHeatedBed = Printer.HasHeatedBed;
            newPrinter.HasFilamentRunoutSensor = Printer.HasFilamentRunoutSensor;
            newPrinter.ZMaxBuildVolume = Printer.ZMaxBuildVolume;
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
            SavePrinterData(PRINTER_DATA_PATH, Printers);
        }

        public void CreatePreheatProfile()
        {
            preheatingProfiles.Add(Printer.PreheatingProfiles);
            Printer.PreheatingProfiles = new PreheatingProfiles();
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
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
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
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
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildMaxFeedrateCommand(Printer.MotionSettings));
        }

        public void SaveToEEPROM()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildSaveToEEPROMCommand());
        }

        public void SavePrinterData<T>(string filePath, List<T> list)
        {
            var jsonString = JsonSerializer.Serialize(list);
            File.WriteAllText(filePath, jsonString);
        }

        public void SavePrinterData<T>(string filePath, T data)
        {
            var jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonString);
        }

        public List<T> LoadPrinterDataList<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<T>>(jsonString);
            }
            return new List<T>();
        }
        public T LoadPrinterData<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            return default(T);
        }
    }
}
