using System.Text.Json;
using System.Text.RegularExpressions;
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
        const string _sTEPS_PER_UNIT_PATTERN = @"M92 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _mAX_FEEDRATES_PATTERN = @"M203 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _mAX_ACCELERATIONS_PATTERN = @"M201 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _pRINT_RETRACT_TRAVEL_ACCELERATION_PATTERN = @"M204 P(\d+\.?\d*) R(\d+\.?\d*) T(\d+\.?\d*)";
        const string _aDVANCED_SETTINGS_PATTERN = @"M205 B(\d+\.?\d*) S(\d+\.?\d*) T(\d+\.?\d*) J(\d+\.?\d*)";
        const string _oFFSET_SETTINGS_PATTERN = @"M206 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*)";
        const string _aUTO_BED_LEVELING_PATTERN = @"M420 S(\d) Z(\d+\.?\d*)";
        const string _z_PROBE_OFFSETS_PATTERN = @"M851 X(-?\d+\.?\d*) Y(-?\d+\.?\d*) Z(-?\d+\.?\d*)";

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
        public string linearUnits = "";
        public string temperatureUnits = "";

        public PrinterDataServiceTest(ISnackbar snackbar)
        {
            Printer = new Printer();
            _snackbar = snackbar;
            Printer.HotendTemperatures = new HotendTemps(Printer.PrinterConnection);
            Printer.BedTemperatures = new BedTemps(Printer.PrinterConnection);
            Printer.ChamberTemperatures = new ChamberTemps(Printer.PrinterConnection);
            Printer.Camera = new Camera();
            Printer.Head = new PrinterHead();
            Printer.PreheatingProfiles = new PreheatingProfiles();
            Printer.MotionSettings = new MotionSettingsData();
            Printer.PrintHistory = new PrintHistory();
            Printer.CurrentPrintJob = new CurrentPrintJob(Printer.PrinterConnection);
            Printer.TouchScreen = new TouchScreen();
            Printer.HotendTemperatures.PIDHotendValues = new PIDValues(Printer.PrinterConnection);
            Printer.BedTemperatures.PIDBedValues = new PIDValues(Printer.PrinterConnection);
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


        public void GetPrinterSettings()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildReportSettings());
            Thread.Sleep(400);
            string response = ConnectionServiceSerial.printerConnection.Read();
            linearUnits = GetPrinterLinearUnits(response);
            temperatureUnits = GetTemperatureUnits(response);
            GetStepsPerUnit(response);
            GetMaximumFeedrates(response);
            GetMaximumAccelerations(response);
            GetPrintRetractTravelAcceleration(response);
            GetStartingAccelerations(response);
            GetOffsetSettings(response);
            GetAutoBedLevelingSettings(response);
            GetHotendPIDValues(response);
            GetBedPIDValues(response);
            GetZProbeOffsets(response);
        }


        public string GetPrinterLinearUnits(string input)
        {
            var linearUnits = input.Split('\n').Where(x => x.Contains("G21") || x.Contains("G20")).FirstOrDefault();
            if (linearUnits.Contains("G21"))
            {
                return "mm";
            }
            else if (linearUnits.Contains("G20"))
            {
                return "inches";
            }
            return null;
        }

        public string GetTemperatureUnits(string input)
        {
            var match = Regex.Match(input, @"M149 ([CFK])");

            if (match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "C":
                        return "Celsius";
                    case "F":
                        return "Farenheit";
                    case "K":
                        return "Kelvin";
                }
            }
            return "Not Found";
        }

        public void GetStepsPerUnit(string input)
        {
            var match = Regex.Match(input, _sTEPS_PER_UNIT_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.XStepsPerUnit = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.YStepsPerUnit = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.ZStepsPerUnit = (int)double.Parse(match.Groups[3].Value);
                Printer.MotionSettings.EStepsPerUnit = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetMaximumFeedrates(string input)
        {
            var match = Regex.Match(input, _mAX_FEEDRATES_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.XMaxFeedrate = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.YMaxFeedrate = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.ZMaxFeedrate = (int)double.Parse(match.Groups[3].Value);
                Printer.MotionSettings.EMaxFeedrate = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetMaximumAccelerations(string input)
        {
            var match = Regex.Match(input, _mAX_ACCELERATIONS_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.XMaxAcceleration = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.YMaxAcceleration = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.ZMaxAcceleration = (int)double.Parse(match.Groups[3].Value);
                Printer.MotionSettings.EMaxAcceleration = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetPrintRetractTravelAcceleration(string input)
        {
            var match = Regex.Match(input, _pRINT_RETRACT_TRAVEL_ACCELERATION_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.PrintAcceleration = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.RetractAcceleration = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.TravelAcceleration = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetStartingAccelerations(string input)
        {
            var match = Regex.Match(input, _aDVANCED_SETTINGS_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.MinSegmentTime = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.MinFeedrate = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.MinTravelFeedrate = (int)double.Parse(match.Groups[3].Value);
                Printer.MotionSettings.JunctionDeviation = double.Parse(match.Groups[4].Value);
            }
        }

        public void GetOffsetSettings(string input)
        {
            var match = Regex.Match(input, _oFFSET_SETTINGS_PATTERN);

            if (match.Success)
            {
                Printer.MotionSettings.XHomeOffset = (int)double.Parse(match.Groups[1].Value);
                Printer.MotionSettings.YHomeOffset = (int)double.Parse(match.Groups[2].Value);
                Printer.MotionSettings.ZHomeOffset = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetAutoBedLevelingSettings(string input)
        {
            var match = Regex.Match(input, _aUTO_BED_LEVELING_PATTERN);
            if (match.Success)
            {
                Printer.HasAutoBedLevel = (int)double.Parse(match.Groups[1].Value) == 1;

                Printer.MotionSettings.FadeHeight = (int)double.Parse(match.Groups[2].Value);
            }
        }

        public void GetHotendPIDValues(string input)
        {
            var match = Regex.Match(input, PIDValues.PARSE_HOTEND_PID_PATTERN);
            if (match.Success)
            {
                Printer.HotendTemperatures.PIDHotendValues.Proportional = double.Parse(match.Groups[1].Value);
                Printer.HotendTemperatures.PIDHotendValues.Integral = double.Parse(match.Groups[2].Value);
                Printer.HotendTemperatures.PIDHotendValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetBedPIDValues(string input)
        {
            var match = Regex.Match(input, PIDValues.PARSE_BED_PID_PATTERN);
            if (match.Success)
            {
                Printer.BedTemperatures.PIDBedValues.Proportional = double.Parse(match.Groups[1].Value);
                Printer.BedTemperatures.PIDBedValues.Integral = double.Parse(match.Groups[2].Value);
                Printer.BedTemperatures.PIDBedValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetZProbeOffsets(string input)
        {
            var match = Regex.Match(input, _z_PROBE_OFFSETS_PATTERN);
            if (match.Success)
            {
                Printer.Head.XProbeOffset = double.Parse(match.Groups[1].Value);
                Printer.Head.YProbeOffset = double.Parse(match.Groups[2].Value);
                Printer.Head.ZProbeOffset = double.Parse(match.Groups[3].Value);
            }
        }

    }
}