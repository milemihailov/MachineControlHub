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
        public const string PRINT_HISTORY_PATH = "printHistory.json";

        const string _sTEPS_PER_UNIT_PATTERN = @"M92 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _mAX_FEEDRATES_PATTERN = @"M203 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _mAX_ACCELERATIONS_PATTERN = @"M201 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*) E(\d+\.?\d*)";
        const string _pRINT_RETRACT_TRAVEL_ACCELERATION_PATTERN = @"M204 P(\d+\.?\d*) R(\d+\.?\d*) T(\d+\.?\d*)";
        const string _aDVANCED_SETTINGS_PATTERN = @"M205 B(\d+\.?\d*) S(\d+\.?\d*) T(\d+\.?\d*) J(\d+\.?\d*)";
        const string _oFFSET_SETTINGS_PATTERN = @"M206 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*)";
        const string _aUTO_BED_LEVELING_PATTERN = @"M420 S(\d) Z(\d+\.?\d*)";
        const string _z_PROBE_OFFSETS_PATTERN = @"M851 X(-?\d+\.?\d*) Y(-?\d+\.?\d*) Z(-?\d+\.?\d*)";
        const string _bED_VOLUME_PATTERN = @"Max\s*:\s*X(\d+\.?\d*)\s*Y(\d+\.?\d*)\s*Z(\d+\.?\d*)";



        public Printer Printer = new();
        public ConnectionServiceSerial serialConnection;
        public readonly BackgroundTimer Background;
        public readonly HotendTemperatureService HotendTemperatureService;
        public readonly BedTemperatureService BedTemperatureService;
        public readonly ChamberTemperatureService ChamberTemperatureService;
        public readonly BedLevelingService BedLevelDataService;
        private readonly ISnackbar _snackbar;

        public bool IsConnected { get; set; }

        public List<PreheatingProfiles> preheatingProfiles = new List<PreheatingProfiles>();
        public List<CurrentPrintJob> printHistory = new List<CurrentPrintJob>();
        public List<Printer> Printers { get; set; }
        public List<PrinterHead> Extruders { get; set; }
        public List<PrinterHead> ExtruderSettings = new List<PrinterHead>();
        public PrinterBed Bed { get; set; }

        public Printer SelectedPrinter;

        public PrinterDataServiceTest(BackgroundTimer background, ISnackbar snackbar)
        {
            this.Background = background;
            _snackbar = snackbar;
            Printer = new Printer();
            if (background.ConnectionServiceSerial != null)
            {
                HotendTemperatureService = new HotendTemperatureService(snackbar, background);
                BedTemperatureService = new BedTemperatureService(snackbar, background);
                ChamberTemperatureService = new ChamberTemperatureService(snackbar, background);
                HotendTemperatureService.PIDHotendValues = new PIDValues(background.ConnectionServiceSerial.printerConnection);
                BedTemperatureService.PIDBedValues = new PIDValues(background.ConnectionServiceSerial.printerConnection);
                BedLevelDataService = new BedLevelingService();
            }

            Printer.Camera = new Camera();
            Printer.Head = new PrinterHead();
            Printer.PreheatingProfiles = new PreheatingProfiles();
            Printer.MotionSettings = new MotionSettingsData();
            Printer.StepperDrivers = new StepperDriversData();
            Printer.PrintHistory = new PrintHistory();
            Printer.TouchScreen = new TouchScreen();
            Extruders = new List<PrinterHead>();
            Printer.Bed = new PrinterBed();
            Printers = new List<Printer>();
            SelectedPrinter = new Printer();
            background.MessageReceived += (message) => _ = OnUpdateSettings(message);
        }

        //public void ChoosePrinter(Printer printer)
        //{
        //    SelectedPrinter = printer;
        //    SavePrinterData(SELECTED_PRINTER_SETTINGS_PATH, SelectedPrinter);
        //}

        //public void RemovePrinter(Printer printer)
        //{
        //    Printers.Remove(printer);
        //    SavePrinterData(PRINTER_DATA_PATH, Printers);
        //}

        //public void UpdateExtruders(int newValue)
        //{
        //    while (ExtruderSettings.Count < newValue)
        //    {
        //        ExtruderSettings.Add(new PrinterHead());
        //    }
        //    while (ExtruderSettings.Count > newValue)
        //    {
        //        ExtruderSettings.RemoveAt(ExtruderSettings.Count - 1);
        //    }
        //}

        public void AddMe()
        {
            foreach (var connection in Background.ConnectionsHistory)
            {
                Console.WriteLine(connection);
            }
        }

        public void AddPrintJobToHistory(CurrentPrintJob currentPrintJob)
        {
            var newPrintJob = new CurrentPrintJob(Background.ConnectionServiceSerial.printerConnection)
            {
                FileName = currentPrintJob.FileName,
                TotalPrintTime = currentPrintJob.TotalPrintTime,
                StartTimeOfPrint = currentPrintJob.StartTimeOfPrint,
                FileSize = currentPrintJob.FileSize
            };
            printHistory.Insert(0, newPrintJob);
            SavePrinterData(PRINT_HISTORY_PATH, printHistory);
        }

        //public string GenerateUniquePrinterId()
        //{
        //    Random random = new Random();
        //    string newId;
        //    do
        //    {
        //        newId = $"#{random.Next(1, 10000)}";
        //    }
        //    while (Printers.Any(p => p.Id == newId));

        //    return newId;
        //}

        //public void CreatePrinterProfile()
        //{
        //var newPrinter = new Printer();
        //    newPrinter.Extruders = new List<PrinterHead>();
        //    newPrinter.Bed = new PrinterBed();
        //    newPrinter.Name = Printer.Name;
        //    newPrinter.Id = GenerateUniquePrinterId();
        //    newPrinter.Model = Printer.Model;
        //    newPrinter.NumberOfExtruders = Printer.NumberOfExtruders;
        //    newPrinter.PrinterFirmwareVersion = Printer.PrinterFirmwareVersion;
        //    newPrinter.HasAutoBedLevel = Printer.HasAutoBedLevel;
        //    newPrinter.HasChamber = Printer.HasChamber;
        //    newPrinter.HasHeatedBed = Printer.HasHeatedBed;
        //    newPrinter.HasFilamentRunoutSensor = Printer.HasFilamentRunoutSensor;
        //    newPrinter.ZMaxBuildVolume = Printer.ZMaxBuildVolume;
        //    newPrinter.Bed.XSize = Printer.Bed.XSize;
        //    newPrinter.Bed.YSize = Printer.Bed.YSize;
        //    newPrinter.Bed.Diameter = Printer.Bed.Diameter;


        //    for (int i = 0; i < ExtruderSettings.Count; i++)
        //    {
        //        var newExtruder = new PrinterHead();

        //        newExtruder.HasCoolingFan = ExtruderSettings[i].HasCoolingFan;
        //        newExtruder.ProbePresent = ExtruderSettings[i].ProbePresent;
        //        newExtruder.NozzleDiameter = ExtruderSettings[i].NozzleDiameter;
        //        newExtruder.NozzleMaterial = ExtruderSettings[i].NozzleMaterial;

        //        newPrinter.Extruders.Add(newExtruder);
        //    }
        //    Printers.Add(newPrinter);
        //SavePrinterData(PRINTER_DATA_PATH, Printers);
        //}

        public void CreatePreheatProfile()
        {
            preheatingProfiles.Add(Printer.PreheatingProfiles);
            Printer.PreheatingProfiles = new PreheatingProfiles();
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
            _snackbar.Add("Preset Added To Print Section", Severity.Success);
        }

        public void StartPreheating(PreheatingProfiles profile)
        {
            Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetHotendTempCommand(profile.HotendTemp));
            Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetBedTempCommand(profile.BedTemp));
            Background.ConnectionServiceSerial.Write(CommandMethods.BuildFanSpeedCommand(profile.FanSpeed));
            _snackbar.Add("Preheating Started", Severity.Success);
        }

        public void DeletePreheatingProfile(PreheatingProfiles profile)
        {
            preheatingProfiles.Remove(profile);
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
            _snackbar.Add("Preset Removed", Severity.Error);
        }

        public void SaveToEEPROM()
        {
            Background.ConnectionServiceSerial.Write(CommandMethods.BuildSaveToEEPROMCommand());
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
            Background.ConnectionServiceSerial.Write(CommandMethods.BuildReportSettings());
        }

        public void GetFirmwareSettings()
        {
            Background.ConnectionServiceSerial.Write("M115");
            Background.ConnectionServiceSerial.Write("M569 ");
        }

        public void GetBedVolume()
        {
            Background.ConnectionServiceSerial.Write("M211");
        }

        public async Task OnUpdateSettings(string message)
        {
            await Task.Run(() =>
            {
                if (message.Contains("G21") || message.Contains("G20"))
                {
                    Printer.LinearUnit = GetPrinterLinearUnits(message);
                }
                if (message.Contains("M149"))
                {
                    Printer.TemperatureUnit = GetTemperatureUnits(message);
                }

                GetStepsPerUnit(message);
                GetMaximumFeedrates(message);
                GetMaximumAccelerations(message);
                GetPrintRetractTravelAcceleration(message);
                GetStartingAccelerations(message);
                GetOffsetSettings(message);
                GetAutoBedLevelingSettings(message);
                GetHotendPIDValues(message);
                GetBedPIDValues(message);
                GetZProbeOffsets(message);
                GetFirmwareVersion(message);
                GetBinaryFileTransferState(message);
                GetAutoReportState(message);
                GetExtruderCount(message);
                GetEEPROMState(message);
                GetFilamentRunoutSensorState(message);
                GetAutoLevelState(message);
                GetZProbeState(message);
                GetSoftwarePowerState(message);
                GetEmergencyParserState(message);
                GetToggleLightsState(message);
                GetHostActionCommandsState(message);
                GetPromptSupportState(message);
                GetSDCardSupportState(message);
                GetLongFilenameSupportState(message);
                GetCustomFirmwareUploadState(message);
                GetExtendedM20Support(message);
                GetThermalProtectionState(message);
                GetBabyStepState(message);
                GetPowerLossRecoveryState(message);
                GetStepperDriverCurrents(message);
                GetStepperDriverMode(message);
                GetHomingSensitivityValues(message);
            });
            await SetBedVolume(message);
        }


        public string GetPrinterLinearUnits(string input)
        {
            var linearUnits = input.Split('\n').Where(x => x.Contains("G21") || x.Contains("G20")).FirstOrDefault();
            if (linearUnits.Contains("G21"))
            {
                return "Milimeters";
            }
            else if (linearUnits.Contains("G20"))
            {
                return "Inches";
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
                Printer.MotionSettings.MinPrintFeedrate = (int)double.Parse(match.Groups[2].Value);
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
                HotendTemperatureService.PIDHotendValues.Proportional = double.Parse(match.Groups[1].Value);
                HotendTemperatureService.PIDHotendValues.Integral = double.Parse(match.Groups[2].Value);
                HotendTemperatureService.PIDHotendValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetBedPIDValues(string input)
        {
            var match = Regex.Match(input, PIDValues.PARSE_BED_PID_PATTERN);
            if (match.Success)
            {
                BedTemperatureService.PIDBedValues.Proportional = double.Parse(match.Groups[1].Value);
                BedTemperatureService.PIDBedValues.Integral = double.Parse(match.Groups[2].Value);
                BedTemperatureService.PIDBedValues.Derivative = double.Parse(match.Groups[3].Value);
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

        public void GetFirmwareVersion(string message)
        {
            var match = Regex.Match(message, @"FIRMWARE_NAME:([^ ]* [^ ]*)");
            if (match.Success)
            {
                Printer.PrinterFirmwareVersion = match.Groups[1].Value;

            }
        }

        public void GetExtruderCount(string message)
        {
            var match = Regex.Match(message, @"EXTRUDER_COUNT:(\d+)");
            if (match.Success)
            {
                Printer.NumberOfExtruders = int.Parse(match.Groups[1].Value);
            }
        }

        public void GetStepperDriverCurrents(string message)
        {
            var match = Regex.Match(message, @"M906 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*)");
            if (match.Success)
            {
                Printer.StepperDrivers.XDriverCurrent = double.Parse(match.Groups[1].Value);
                Printer.StepperDrivers.YDriverCurrent = double.Parse(match.Groups[2].Value);
                Printer.StepperDrivers.ZDriverCurrent = double.Parse(match.Groups[3].Value);
            }

            var matchE = Regex.Match(message, @"M906 T(\d+) E(\d+\.?\d*)");
            if (matchE.Success)
            {
                int extruderIndex = int.Parse(matchE.Groups[1].Value);
                double current = double.Parse(matchE.Groups[2].Value);

                switch (extruderIndex)
                {
                    case 0:
                        Printer.StepperDrivers.EDriverCurrent = current;
                        break;
                    case 1:
                        Printer.StepperDrivers.E2DriverCurrent = current;
                        break;
                    case 2:
                        Printer.StepperDrivers.E3DriverCurrent = current;
                        break;
                    case 3:
                        Printer.StepperDrivers.E4DriverCurrent = current;
                        break;
                    case 4:
                        Printer.StepperDrivers.E5DriverCurrent = current;
                        break;
                    default:
                        Console.WriteLine("Invalid extruder index");
                        break;
                }
            }

            var matchMultpleZ = Regex.Match(message, @"M906 I([1-9]\d*) Z(\d+\.?\d*)");
            if (matchMultpleZ.Success)
            {
                int zDriverIndex = int.Parse(matchMultpleZ.Groups[1].Value);
                double current = double.Parse(matchMultpleZ.Groups[2].Value);

                switch (zDriverIndex)
                {
                    case 1:
                        Printer.StepperDrivers.Z2DriverCurrent = current;
                        break;
                    case 2:
                        Printer.StepperDrivers.Z3DriverCurrent = current;
                        break;
                    default:
                        Console.WriteLine("Invalid Z driver index");
                        break;
                }
            }
        }

        public void GetStepperDriverMode(string message)
        {
            var match = Regex.Match(message, @"(\w+)\s+driver mode:\s+(\w+)");

            if (match.Success)
            {
                var propertyMap = new Dictionary<string, Action<string>>
                    {
                        { "X", value => Printer.StepperDrivers.XDriverSteppingMode = value },
                        { "Y", value => Printer.StepperDrivers.YDriverSteppingMode = value },
                        { "Z", value => Printer.StepperDrivers.ZDriverSteppingMode = value },
                        { "Z2", value => Printer.StepperDrivers.Z2DriverSteppingMode = value },
                        { "Z3", value => Printer.StepperDrivers.Z3DriverSteppingMode = value },
                        { "E", value => Printer.StepperDrivers.EDriverSteppingMode = value },
                        { "E2", value => Printer.StepperDrivers.E2DriverSteppingMode = value },
                    };

                string group1 = match.Groups[1].Value;
                string group2 = match.Groups[2].Value;

                if (propertyMap.ContainsKey(group1))
                {
                    propertyMap[group1].Invoke(group2);
                }
            }
        }

        public void GetHomingSensitivityValues(string message)
        {
            var match = Regex.Match(message, @"M914 X(\d+\.?\d*) Y(\d+\.?\d*)(?: Z(\d+\.?\d*))?");

            if (match.Success)
            {
                Printer.StepperDrivers.XStallGuardTreshold = double.Parse(match.Groups[1].Value);
                Printer.StepperDrivers.YStallGuardTreshold = double.Parse(match.Groups[2].Value);
                Printer.StepperDrivers.ZStallGuardTreshold = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetBinaryFileTransferState(string message)
        {
            var match = Regex.Match(message, @"BINARY_FILE_TRANSFER:(\d+)");
            if (match.Success)
            {
                Printer.HasBinaryFileTransfer = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoReportState(string message)
        {
            var matchPosition = Regex.Match(message, @"AUTOREPORT_POS:(\d+)");
            if (matchPosition.Success)
            {
                Printer.HasAutoReportPosition = BoolOutput(int.Parse(matchPosition.Groups[1].Value));
            }

            var matchTemperature = Regex.Match(message, @"AUTOREPORT_TEMP:(\d+)");
            if (matchTemperature.Success)
            {
                Printer.HasAutoReportTemperature = BoolOutput(int.Parse(matchTemperature.Groups[1].Value));
            }

            var matchSDStatus = Regex.Match(message, @"AUTOREPORT_SD_STATUS:(\d+)");
            if (matchSDStatus.Success)
            {
                Printer.HasAutoReportSDStatus = BoolOutput(int.Parse(matchSDStatus.Groups[1].Value));
            }
        }

        public void GetEEPROMState(string message)
        {
            var match = Regex.Match(message, @"EEPROM:(\d+)");
            if (match.Success)
            {
                Printer.HasEEPROM = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetFilamentRunoutSensorState(string message)
        {
            var match = Regex.Match(message, @"RUNOUT:(\d+)");
            if (match.Success)
            {
                Printer.HasFilamentRunoutSensor = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoLevelState(string message)
        {
            var match = Regex.Match(message, @"AUTOLEVEL:(\d+)");
            if (match.Success)
            {
                Printer.HasAutoBedLevel = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetZProbeState(string message)
        {
            var match = Regex.Match(message, @"Z_PROBE:(\d+)");
            if (match.Success)
            {
                Printer.Head.ProbePresent = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSoftwarePowerState(string message)
        {
            var match = Regex.Match(message, @"SOFTWARE_POWER:(\d+)");
            if (match.Success)
            {
                Printer.HasSoftwarePowerControl = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetEmergencyParserState(string message)
        {
            var match = Regex.Match(message, @"EMERGENCY_PARSER:(\d+)");
            if (match.Success)
            {
                Printer.HasEmergencyParser = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetToggleLightsState(string message)
        {
            var match = Regex.Match(message, @"TOGGLE_LIGHTS:(\d+)");
            if (match.Success)
            {
                Printer.HasToggleLights = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetHostActionCommandsState(string message)
        {
            var match = Regex.Match(message, @"HOST_ACTION_COMMANDS:(\d+)");
            if (match.Success)
            {
                Printer.HasHostActionCommands = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPromptSupportState(string message)
        {
            var match = Regex.Match(message, @"PROMPT_SUPPORT:(\d+)");
            if (match.Success)
            {
                Printer.HasPromptSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSDCardSupportState(string message)
        {
            var match = Regex.Match(message, @"SDCARD:(\d+)");
            if (match.Success)
            {
                Printer.HasSDCardSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetLongFilenameSupportState(string message)
        {
            var match = Regex.Match(message, @"LONG_FILENAME:(\d+)");
            if (match.Success)
            {
                Printer.HasLongFilenameSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetCustomFirmwareUploadState(string message)
        {
            var match = Regex.Match(message, @"CUSTOM_FIRMWARE_UPLOAD:(\d+)");
            if (match.Success)
            {
                Printer.HasCustomFirmwareUpload = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetExtendedM20Support(string message)
        {
            var match = Regex.Match(message, @"EXTENDED_M20:(\d+)");
            if (match.Success)
            {
                Printer.HasExtendedM20 = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetThermalProtectionState(string message)
        {
            var match = Regex.Match(message, @"THERMAL_PROTECTION:(\d+)");
            if (match.Success)
            {
                Printer.HasThermalProtection = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetBabyStepState(string message)
        {
            var match = Regex.Match(message, @"BABYSTEPPING:(\d+)");
            if (match.Success)
            {
                Printer.HasBabystep = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPowerLossRecoveryState(string message)
        {
            var match = Regex.Match(message, @"M413 S(\d+)");
            if (match.Success)
            {
                Printer.HasPowerLossRecovery = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public async Task SetBedVolume(string input)
        {
            await Task.Run(() =>
            {
                if (input != null)
                {
                    var match = Regex.Match(input, _bED_VOLUME_PATTERN, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        Printer.Bed.XSize = (int)double.Parse(match.Groups[1].Value);
                        Printer.Bed.YSize = (int)double.Parse(match.Groups[2].Value);
                    }
                }
            });
        }

        public void SetMaximumFeedrates()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildMaxFeedrateCommand(Printer.MotionSettings));
            }
        }

        public void SetStepsPerUnit()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetStepsPerUnitCommand(Printer.MotionSettings));

            }
        }

        public void SetStartingAccelerations()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetStartingAccelerationCommand(Printer.MotionSettings));
            }
        }

        public void SetMaximumAccelerations()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetMaxAccelerationCommand(Printer.MotionSettings));
            }
        }

        public void SetAdvancedSettings()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetAdvancedSettingsCommand(Printer.MotionSettings));
            }
        }

        public void SetOffsetSettings()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetHomeOffsetsCommand(Printer.MotionSettings));
            }
        }


        public void SetBedLevelingOn()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                if (Printer.HasAutoBedLevel)
                {
                    Background.ConnectionServiceSerial.Write("M420 S0");
                    _snackbar.Add("Bed leveling turned off", Severity.Warning);
                }
                else
                {
                    Background.ConnectionServiceSerial.Write("M420 S1");
                    _snackbar.Add("Bed leveling turned on", Severity.Success);
                }
            }
        }
        public void SetFadeHeight()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write($"M420 Z{Printer.MotionSettings.FadeHeight}");
                _snackbar.Add($"Fade height set to {Printer.MotionSettings.FadeHeight}mm", Severity.Success);
            }
        }

        public void SetPreheatingProfiles()
        {
            preheatingProfiles.ForEach(profile =>
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildMaterialPresetCommand(profile));
                _snackbar.Add("Preset added to printer", Severity.Success);
            });
        }

        public void SetDriverCurrents()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetDriverCurrentsCommand(Printer.StepperDrivers));
            }
        }

        public void SetDriverSteppingMode()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetDriverSteppingMode(Printer.StepperDrivers));
            }
        }

        public void SetBumpSensitivty()
        {
            if (Background.ConnectionServiceSerial.printerConnection.IsConnected)
            {
                Background.ConnectionServiceSerial.Write(CommandMethods.BuildSetBumpSensitivity(Printer.StepperDrivers));
            }
        }

        public bool BoolOutput(int input)
        {
            return input == 1 ? true : false;
        }

        public string StringOutput(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}