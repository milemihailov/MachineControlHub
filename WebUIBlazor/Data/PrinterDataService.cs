using System.Text.Json;
using System.Text.RegularExpressions;
using MachineControlHub;
using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Print;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class PrinterDataService
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


        public List<PreheatingProfiles> preheatingProfiles { get; set; } = new List<PreheatingProfiles>();
        public List<CurrentPrintJob> printHistory { get; set; } = new List<CurrentPrintJob>();
        public PrinterManagerService PrinterManagerService { get; set; }

        public PrinterDataService(PrinterManagerService Printer)
        {
            PrinterManagerService = Printer;
            Printer.ActivePrinter.PreheatingProfiles = new PreheatingProfiles();
            Printer.ActivePrinter.PrintHistory = new PrintHistory();
        }


        public void AddPrintJobToHistory(CurrentPrintJob currentPrintJob, PrinterManagerService printerManager)
        {
            var newPrintJob = new CurrentPrintJob(PrinterManagerService.ActivePrinter.SerialConnection)
            {
                PortName = printerManager.ActivePrinter.SerialConnection.PortName,
                PrinterName = printerManager.ActivePrinter.Name,
                FileName = currentPrintJob.FileName,
                TotalPrintTime = currentPrintJob.TotalPrintTime,
                StartTimeOfPrint = currentPrintJob.StartTimeOfPrint,
                FileSize = currentPrintJob.FileSize
            };

            // Deserialize the print history list
            printHistory = LoadPrinterDataList<CurrentPrintJob>(PRINT_HISTORY_PATH);
            // Add the new print job to the top of the list
            printHistory.Insert(0, newPrintJob);
            // Serialize updated print history list
            SavePrinterData(PRINT_HISTORY_PATH, printHistory);
        }

        public void RemovePrintJob(CurrentPrintJob printJob)
        {
            printHistory.Remove(printJob);
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

        public void CreatePreheatProfile()
        {
            preheatingProfiles.Add(PrinterManagerService.ActivePrinter.PreheatingProfiles);
            PrinterManagerService.ActivePrinter.PreheatingProfiles = new PreheatingProfiles();
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
        }

        public void StartPreheating(PreheatingProfiles profile)
        {
            PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetHotendTempCommand(profile.HotendTemp));
            PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetBedTempCommand(profile.BedTemp));
            PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildFanSpeedCommand(profile.FanSpeed));
        }

        public void DeletePreheatingProfile(PreheatingProfiles profile)
        {
            preheatingProfiles.Remove(profile);
            SavePrinterData(PREHEATING_PROFILES_PATH, preheatingProfiles);
        }

        public void SaveToEEPROM()
        {
            PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSaveToEEPROMCommand());
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
            PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildReportSettings());
        }


        public void GetFirmwareSettings()
        {
            PrinterManagerService.ActivePrinter.SerialConnection.Write("M115");
            PrinterManagerService.ActivePrinter.SerialConnection.Write("M569");
            PrinterManagerService.ActivePrinter.SerialConnection.Write("M78");
        }

        public void SetSoftwareEndstopsState(string input)
        {
            PrinterManagerService.ActivePrinter.SerialConnection.Write($"M211 S{input}");
        }

        public async void OnUpdateSettings(string message)
        {
            await Task.Run(() =>
            {
                if (message.Contains("G21") || message.Contains("G20"))
                {
                    PrinterManagerService.ActivePrinter.LinearUnit = GetPrinterLinearUnits(message);
                }
                if (message.Contains("M149"))
                {
                    PrinterManagerService.ActivePrinter.TemperatureUnit = GetTemperatureUnits(message);
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
                GetPrintJobStats(message);
                SetBedVolume(message);
            });
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
                PrinterManagerService.ActivePrinter.MotionSettings.XStepsPerUnit = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.YStepsPerUnit = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.ZStepsPerUnit = (int)double.Parse(match.Groups[3].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.EStepsPerUnit = (int)double.Parse(match.Groups[4].Value);
                Console.WriteLine($"Selected Printer: {PrinterManagerService.ActivePrinter.Name} X Steps:{PrinterManagerService.ActivePrinter.MotionSettings.XStepsPerUnit}");
            }
        }

        public void GetMaximumFeedrates(string input)
        {
            var match = Regex.Match(input, _mAX_FEEDRATES_PATTERN);

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.MotionSettings.XMaxFeedrate = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.YMaxFeedrate = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.ZMaxFeedrate = (int)double.Parse(match.Groups[3].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.EMaxFeedrate = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetMaximumAccelerations(string input)
        {
            var match = Regex.Match(input, _mAX_ACCELERATIONS_PATTERN);

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.MotionSettings.XMaxAcceleration = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.YMaxAcceleration = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.ZMaxAcceleration = (int)double.Parse(match.Groups[3].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.EMaxAcceleration = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetPrintRetractTravelAcceleration(string input)
        {
            var match = Regex.Match(input, _pRINT_RETRACT_TRAVEL_ACCELERATION_PATTERN);

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.MotionSettings.PrintAcceleration = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.RetractAcceleration = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.TravelAcceleration = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetStartingAccelerations(string input)
        {
            var match = Regex.Match(input, _aDVANCED_SETTINGS_PATTERN);

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.MotionSettings.MinSegmentTime = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.MinPrintFeedrate = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.MinTravelFeedrate = (int)double.Parse(match.Groups[3].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.JunctionDeviation = double.Parse(match.Groups[4].Value);
            }
        }

        public void GetOffsetSettings(string input)
        {
            var match = Regex.Match(input, _oFFSET_SETTINGS_PATTERN);

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.MotionSettings.XHomeOffset = (int)double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.YHomeOffset = (int)double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.MotionSettings.ZHomeOffset = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetAutoBedLevelingSettings(string input)
        {
            var match = Regex.Match(input, _aUTO_BED_LEVELING_PATTERN);
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasAutoBedLevel = (int)double.Parse(match.Groups[1].Value) == 1;
                PrinterManagerService.ActivePrinter.MotionSettings.FadeHeight = (int)double.Parse(match.Groups[2].Value);
            }
        }

        public void GetHotendPIDValues(string input)
        {
            var match = Regex.Match(input, PIDValues.PARSE_HOTEND_PID_PATTERN);
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HotendTemperatures.PIDValues.Proportional = double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.HotendTemperatures.PIDValues.Integral = double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.HotendTemperatures.PIDValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetBedPIDValues(string input)
        {
            var match = Regex.Match(input, PIDValues.PARSE_BED_PID_PATTERN);
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.BedTemperatures.PIDValues.Proportional = double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.BedTemperatures.PIDValues.Integral = double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.BedTemperatures.PIDValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetZProbeOffsets(string input)
        {
            var match = Regex.Match(input, _z_PROBE_OFFSETS_PATTERN);
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.Head.XProbeOffset = double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.Head.YProbeOffset = double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.Head.ZProbeOffset = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetFirmwareVersion(string message)
        {
            var match = Regex.Match(message, @"FIRMWARE_NAME:([^ ]* [^ ]*)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.PrinterFirmwareVersion = match.Groups[1].Value;

            }
        }

        public void GetExtruderCount(string message)
        {
            var match = Regex.Match(message, @"EXTRUDER_COUNT:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.NumberOfExtruders = int.Parse(match.Groups[1].Value);
            }
        }

        public void GetStepperDriverCurrents(string message)
        {
            var match = Regex.Match(message, @"M906 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.StepperDrivers.XDriverCurrent = double.Parse(match.Groups[1].Value);
                PrinterManagerService.ActivePrinter.StepperDrivers.YDriverCurrent = double.Parse(match.Groups[2].Value);
                PrinterManagerService.ActivePrinter.StepperDrivers.ZDriverCurrent = double.Parse(match.Groups[3].Value);
            }

            var matchE = Regex.Match(message, @"M906 T(\d+) E(\d+\.?\d*)");
            if (matchE.Success)
            {
                int extruderIndex = int.Parse(matchE.Groups[1].Value);
                double current = double.Parse(matchE.Groups[2].Value);

                switch (extruderIndex)
                {
                    case 0:
                        PrinterManagerService.ActivePrinter.StepperDrivers.EDriverCurrent = current;
                        break;
                    case 1:
                        PrinterManagerService.ActivePrinter.StepperDrivers.E2DriverCurrent = current;
                        break;
                    case 2:
                        PrinterManagerService.ActivePrinter.StepperDrivers.E3DriverCurrent = current;
                        break;
                    case 3:
                        PrinterManagerService.ActivePrinter.StepperDrivers.E4DriverCurrent = current;
                        break;
                    case 4:
                        PrinterManagerService.ActivePrinter.StepperDrivers.E5DriverCurrent = current;
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
                        PrinterManagerService.ActivePrinter.StepperDrivers.Z2DriverCurrent = current;
                        break;
                    case 2:
                        PrinterManagerService.ActivePrinter.StepperDrivers.Z3DriverCurrent = current;
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
                        { "X", value => PrinterManagerService.ActivePrinter.StepperDrivers.XDriverSteppingMode = value },
                        { "Y", value => PrinterManagerService.ActivePrinter.StepperDrivers.YDriverSteppingMode = value },
                        { "Z", value => PrinterManagerService.ActivePrinter.StepperDrivers.ZDriverSteppingMode = value },
                        { "Z2", value => PrinterManagerService.ActivePrinter.StepperDrivers.Z2DriverSteppingMode = value },
                        { "Z3", value => PrinterManagerService.ActivePrinter.StepperDrivers.Z3DriverSteppingMode = value },
                        { "E", value => PrinterManagerService.ActivePrinter.StepperDrivers.EDriverSteppingMode = value },
                        { "E2", value => PrinterManagerService.ActivePrinter.StepperDrivers.E2DriverSteppingMode = value },
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
            var match = Regex.Match(message, @"M914 X(\d+\.?\d*) Y(\d+\.?\d*)?(?: Z(\d+\.?\d*))?");

            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.StepperDrivers.XStallGuardTreshold = double.Parse(match.Groups[1].Value);

                if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    PrinterManagerService.ActivePrinter.StepperDrivers.YStallGuardTreshold = double.Parse(match.Groups[2].Value);
                }

                if (match.Groups[3].Success && !string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    PrinterManagerService.ActivePrinter.StepperDrivers.ZStallGuardTreshold = double.Parse(match.Groups[3].Value);
                }
            }
        }

        public void GetPrintJobStats(string input)
        {
            if (input.Contains("Stats"))
            {
                string printsPattern = @"Prints:\s*(\d+), Finished:\s*(\d+), Failed:\s*(\d+)";
                string timePattern = @"Total time:\s*(\d+h)?\s*(\d+m)?\s*(\d+s)?,?\s*Longest job:\s*(\d+m)?\s*(\d+s)?";
                string filamentPattern = @"Filament used:\s*([\d.]+m)";

                Match printsMatch = Regex.Match(input, printsPattern);
                Match timeMatch = Regex.Match(input, timePattern);
                Match filamentMatch = Regex.Match(input, filamentPattern);

                if (printsMatch.Success)
                {
                    PrinterManagerService.ActivePrinter.PrintHistory.TotalPrints = int.Parse(printsMatch.Groups[1].Value);
                    PrinterManagerService.ActivePrinter.PrintHistory.TotalFinishedPrints = int.Parse(printsMatch.Groups[2].Value);
                    PrinterManagerService.ActivePrinter.PrintHistory.TotalFailedPrints = int.Parse(printsMatch.Groups[3].Value);
                }

                if (timeMatch.Success)
                {
                    PrinterManagerService.ActivePrinter.PrintHistory.TotalPrintTime = $"{timeMatch.Groups[1].Value}{timeMatch.Groups[2].Value}{timeMatch.Groups[3].Value}";
                    PrinterManagerService.ActivePrinter.PrintHistory.LongestPrintJob = $"{timeMatch.Groups[4].Value}{timeMatch.Groups[5].Value}";
                }

                if (filamentMatch.Success)
                {
                    PrinterManagerService.ActivePrinter.PrintHistory.FilamentUsed = filamentMatch.Groups[1].Value;
                }
            }
        }

        public void GetBinaryFileTransferState(string message)
        {
            var match = Regex.Match(message, @"BINARY_FILE_TRANSFER:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasBinaryFileTransfer = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoReportState(string message)
        {
            var matchPosition = Regex.Match(message, @"AUTOREPORT_POS:(\d+)");
            if (matchPosition.Success)
            {
                PrinterManagerService.ActivePrinter.HasAutoReportPosition = BoolOutput(int.Parse(matchPosition.Groups[1].Value));
            }

            var matchTemperature = Regex.Match(message, @"AUTOREPORT_TEMP:(\d+)");
            if (matchTemperature.Success)
            {
                PrinterManagerService.ActivePrinter.HasAutoReportTemperature = BoolOutput(int.Parse(matchTemperature.Groups[1].Value));
            }

            var matchSDStatus = Regex.Match(message, @"AUTOREPORT_SD_STATUS:(\d+)");
            if (matchSDStatus.Success)
            {
                PrinterManagerService.ActivePrinter.HasAutoReportSDStatus = BoolOutput(int.Parse(matchSDStatus.Groups[1].Value));
            }
        }

        public void GetEEPROMState(string message)
        {
            var match = Regex.Match(message, @"EEPROM:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasEEPROM = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetFilamentRunoutSensorState(string message)
        {
            var match = Regex.Match(message, @"RUNOUT:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasFilamentRunoutSensor = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoLevelState(string message)
        {
            var match = Regex.Match(message, @"AUTOLEVEL:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasAutoBedLevel = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetZProbeState(string message)
        {
            var match = Regex.Match(message, @"Z_PROBE:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.Head.ProbePresent = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSoftwarePowerState(string message)
        {
            var match = Regex.Match(message, @"SOFTWARE_POWER:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasSoftwarePowerControl = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetEmergencyParserState(string message)
        {
            var match = Regex.Match(message, @"EMERGENCY_PARSER:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasEmergencyParser = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetToggleLightsState(string message)
        {
            var match = Regex.Match(message, @"TOGGLE_LIGHTS:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasToggleLights = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetHostActionCommandsState(string message)
        {
            var match = Regex.Match(message, @"HOST_ACTION_COMMANDS:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasHostActionCommands = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPromptSupportState(string message)
        {
            var match = Regex.Match(message, @"PROMPT_SUPPORT:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasPromptSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSDCardSupportState(string message)
        {
            var match = Regex.Match(message, @"SDCARD:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasSDCardSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetLongFilenameSupportState(string message)
        {
            var match = Regex.Match(message, @"LONG_FILENAME:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasLongFilenameSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetCustomFirmwareUploadState(string message)
        {
            var match = Regex.Match(message, @"CUSTOM_FIRMWARE_UPLOAD:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasCustomFirmwareUpload = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetExtendedM20Support(string message)
        {
            var match = Regex.Match(message, @"EXTENDED_M20:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasExtendedM20 = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetThermalProtectionState(string message)
        {
            var match = Regex.Match(message, @"THERMAL_PROTECTION:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasThermalProtection = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetBabyStepState(string message)
        {
            var match = Regex.Match(message, @"BABYSTEPPING:(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasBabystep = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPowerLossRecoveryState(string message)
        {
            var match = Regex.Match(message, @"M413 S(\d+)");
            if (match.Success)
            {
                PrinterManagerService.ActivePrinter.HasPowerLossRecovery = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void SetBedVolume(string input)
        {
            if (input != null)
            {
                var match = Regex.Match(input, _bED_VOLUME_PATTERN, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    PrinterManagerService.ActivePrinter.Bed.XSize = (int)double.Parse(match.Groups[1].Value);
                    PrinterManagerService.ActivePrinter.Bed.YSize = (int)double.Parse(match.Groups[2].Value);
                }
            }
        }

        public void SetMaximumFeedrates()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildMaxFeedrateCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }

        public void SetStepsPerUnit()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetStepsPerUnitCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }

        public void SetStartingAccelerations()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetStartingAccelerationCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }

        public void SetMaximumAccelerations()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetMaxAccelerationCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }

        public void SetAdvancedSettings()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetAdvancedSettingsCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }

        public void SetOffsetSettings()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetHomeOffsetsCommand(PrinterManagerService.ActivePrinter.MotionSettings));
            }
        }


        public void SetBedLevelingOn()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                if (PrinterManagerService.ActivePrinter.HasAutoBedLevel)
                {
                    PrinterManagerService.ActivePrinter.SerialConnection.Write("M420 S0");
                }
                else
                {
                    PrinterManagerService.ActivePrinter.SerialConnection.Write("M420 S1");
                }
            }
        }

        public void SetFadeHeight()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write($"M420 Z{PrinterManagerService.ActivePrinter.MotionSettings.FadeHeight}");
            }
        }

        public void SetPreheatingProfiles()
        {
            preheatingProfiles.ForEach(profile =>
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildMaterialPresetCommand(profile));
            });
        }

        public void SetDriverCurrents()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetDriverCurrentsCommand(PrinterManagerService.ActivePrinter.StepperDrivers));
            }
        }

        public void SetDriverSteppingMode()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetDriverSteppingMode(PrinterManagerService.ActivePrinter.StepperDrivers));
            }
        }

        public void SetBumpSensitivty()
        {
            if (PrinterManagerService.ActivePrinter.SerialConnection.IsConnected)
            {
                PrinterManagerService.ActivePrinter.SerialConnection.Write(CommandMethods.BuildSetBumpSensitivity(PrinterManagerService.ActivePrinter.StepperDrivers));
            }
        }

        public bool BoolOutput(int input)
        {
            return input == 1;
        }

        public string StringOutput(bool input)
        {
            return input ? "Yes" : "No";
        }
    }
}