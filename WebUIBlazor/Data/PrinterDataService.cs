﻿using System.Text.Json;
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
using Plotly.Blazor.Traces.SankeyLib;

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


        public List<PreheatingProfiles> PreheatingProfiles { get; set; } = new List<PreheatingProfiles>();
        public List<CurrentPrintJob> PrintHistory { get; set; } = new List<CurrentPrintJob>();
        public PrinterManagerService PrinterManagerService { get; set; }
        public List<(string Label, string Value)> StepperModes { get; set; } = new();

        public PrinterDataService(PrinterManagerService Printer)
        {
            Printer.ActivePrinter.PreheatingProfiles = new PreheatingProfiles();
            Printer.ActivePrinter.PrintHistory = new PrintHistory();
        }


        public void AddPrintJobToHistory(Printer printer)
        {
            var newPrintJob = new CurrentPrintJob(printer.SerialConnection)
            {
                PortName = printer.SerialConnection.PortName,
                PrinterName = printer.Name,
                FileName = printer.CurrentPrintJob.FileName,
                TotalPrintTime = printer.CurrentPrintJob.TotalPrintTime,
                StartTimeOfPrint = printer.CurrentPrintJob.StartTimeOfPrint,
                FileSize = printer.CurrentPrintJob.FileSize
            };

            // Deserialize the print history list
            PrintHistory = LoadPrinterDataList<CurrentPrintJob>(PRINT_HISTORY_PATH);
            // Add the new print job to the top of the list
            PrintHistory.Insert(0, newPrintJob);
            // Serialize updated print history list
            SavePrinterData(PRINT_HISTORY_PATH, PrintHistory);
        }

        public void RemovePrintJob(CurrentPrintJob printJob)
        {
            PrintHistory.Remove(printJob);
            SavePrinterData(PRINT_HISTORY_PATH, PrintHistory);
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

        public void CreatePreheatProfile(Printer printer)
        {
            PreheatingProfiles.Add(printer.PreheatingProfiles);
            printer.PreheatingProfiles = new PreheatingProfiles();
            SavePrinterData(PREHEATING_PROFILES_PATH, PreheatingProfiles);
        }

        public void DeletePreheatingProfile(PreheatingProfiles profile)
        {
            PreheatingProfiles.Remove(profile);
            SavePrinterData(PREHEATING_PROFILES_PATH, PreheatingProfiles);
        }

        public void StartPreheating(PreheatingProfiles profile, Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildSetHotendTempCommand(profile.HotendTemp));
            printer.SerialConnection.Write(CommandMethods.BuildSetBedTempCommand(profile.BedTemp));
            printer.SerialConnection.Write(CommandMethods.BuildFanSpeedCommand(profile.FanSpeed));
        }

        public void SaveToEEPROM(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildSaveToEEPROMCommand());
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


        public void RequestPrinterSettings(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildReportSettings());
        }


        public void RequestFirmwareSettings(Printer printer)
        {
            printer.SerialConnection.Write("M115");
            printer.SerialConnection.Write("M569");
        }

        public void RequestPrintJobStats(Printer printer)
        {
            printer.SerialConnection.Write("M78");
        }

        public void RequestSoftwareEndstopSettings(string input, Printer printer)
        {
            printer.SerialConnection.Write($"M211 S{input}");
        }

        public async void OnUpdateSettings(string message, Printer printer)
        {
            await Task.Run(() =>
            {
                if (message.Contains("G21") || message.Contains("G20"))
                {
                    printer.LinearUnit = GetPrinterLinearUnits(message);
                }
                if (message.Contains("M149"))
                {
                    printer.TemperatureUnit = GetTemperatureUnits(message);
                }

                GetStepsPerUnit(message, printer);
                GetMaximumFeedrates(message, printer);
                GetMaximumAccelerations(message, printer);
                GetPrintRetractTravelAcceleration(message, printer);
                GetStartingAccelerations(message, printer);
                GetOffsetSettings(message, printer);
                GetAutoBedLevelingSettings(message, printer);
                GetHotendPIDValues(message, printer);
                GetBedPIDValues(message, printer);
                GetZProbeOffsets(message, printer);
                GetFirmwareVersion(message, printer);
                GetBinaryFileTransferState(message, printer);
                GetAutoReportState(message, printer);
                GetExtruderCount(message, printer);
                GetEEPROMState(message, printer);
                GetFilamentRunoutSensorState(message, printer);
                GetAutoLevelState(message, printer);
                GetZProbeState(message, printer);
                GetSoftwarePowerState(message, printer);
                GetEmergencyParserState(message, printer);
                GetToggleLightsState(message, printer);
                GetHostActionCommandsState(message, printer);
                GetPromptSupportState(message, printer);
                GetSDCardSupportState(message, printer);
                GetLongFilenameSupportState(message, printer);
                GetCustomFirmwareUploadState(message, printer);
                GetExtendedM20Support(message, printer);
                GetThermalProtectionState(message, printer);
                GetBabyStepState(message, printer);
                GetPowerLossRecoveryState(message, printer);
                GetStepperDriverCurrents(message, printer);
                GetStepperDriverMode(message, printer);
                GetHomingSensitivityValues(message, printer);
                SetBedVolume(message, printer);
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

        public void GetStepsPerUnit(string input, Printer printer)
        {
            var match = Regex.Match(input, _sTEPS_PER_UNIT_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.XStepsPerUnit = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.YStepsPerUnit = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.ZStepsPerUnit = (int)double.Parse(match.Groups[3].Value);
                printer.MotionSettings.EStepsPerUnit = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetMaximumFeedrates(string input, Printer printer)
        {
            var match = Regex.Match(input, _mAX_FEEDRATES_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.XMaxFeedrate = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.YMaxFeedrate = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.ZMaxFeedrate = (int)double.Parse(match.Groups[3].Value);
                printer.MotionSettings.EMaxFeedrate = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetMaximumAccelerations(string input, Printer printer)
        {
            var match = Regex.Match(input, _mAX_ACCELERATIONS_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.XMaxAcceleration = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.YMaxAcceleration = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.ZMaxAcceleration = (int)double.Parse(match.Groups[3].Value);
                printer.MotionSettings.EMaxAcceleration = (int)double.Parse(match.Groups[4].Value);
            }
        }

        public void GetPrintRetractTravelAcceleration(string input, Printer printer)
        {
            var match = Regex.Match(input, _pRINT_RETRACT_TRAVEL_ACCELERATION_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.PrintAcceleration = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.RetractAcceleration = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.TravelAcceleration = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetStartingAccelerations(string input, Printer printer)
        {
            var match = Regex.Match(input, _aDVANCED_SETTINGS_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.MinSegmentTime = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.MinPrintFeedrate = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.MinTravelFeedrate = (int)double.Parse(match.Groups[3].Value);
                printer.MotionSettings.JunctionDeviation = double.Parse(match.Groups[4].Value);
            }
        }

        public void GetOffsetSettings(string input, Printer printer)
        {
            var match = Regex.Match(input, _oFFSET_SETTINGS_PATTERN);

            if (match.Success)
            {
                printer.MotionSettings.XHomeOffset = (int)double.Parse(match.Groups[1].Value);
                printer.MotionSettings.YHomeOffset = (int)double.Parse(match.Groups[2].Value);
                printer.MotionSettings.ZHomeOffset = (int)double.Parse(match.Groups[3].Value);
            }
        }

        public void GetAutoBedLevelingSettings(string input, Printer printer)
        {
            var match = Regex.Match(input, _aUTO_BED_LEVELING_PATTERN);
            if (match.Success)
            {
                printer.HasAutoBedLevel = (int)double.Parse(match.Groups[1].Value) == 1;
                printer.MotionSettings.FadeHeight = (int)double.Parse(match.Groups[2].Value);
            }
        }

        public void GetHotendPIDValues(string input, Printer printer)
        {
            var match = Regex.Match(input, PIDValues.PARSE_HOTEND_PID_PATTERN);
            if (match.Success)
            {
                printer.HotendTemperatures.PIDValues.Proportional = double.Parse(match.Groups[1].Value);
                printer.HotendTemperatures.PIDValues.Integral = double.Parse(match.Groups[2].Value);
                printer.HotendTemperatures.PIDValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetBedPIDValues(string input, Printer printer)
        {
            var match = Regex.Match(input, PIDValues.PARSE_BED_PID_PATTERN);
            if (match.Success)
            {
                printer.BedTemperatures.PIDValues.Proportional = double.Parse(match.Groups[1].Value);
                printer.BedTemperatures.PIDValues.Integral = double.Parse(match.Groups[2].Value);
                printer.BedTemperatures.PIDValues.Derivative = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetZProbeOffsets(string input, Printer printer)
        {
            var match = Regex.Match(input, _z_PROBE_OFFSETS_PATTERN);
            if (match.Success)
            {
                printer.Head.XProbeOffset = double.Parse(match.Groups[1].Value);
                printer.Head.YProbeOffset = double.Parse(match.Groups[2].Value);
                printer.Head.ZProbeOffset = double.Parse(match.Groups[3].Value);
            }
        }

        public void GetFirmwareVersion(string message, Printer printer)
        {
            var match = Regex.Match(message, @"FIRMWARE_NAME:([^ ]* [^ ]*)");
            if (match.Success)
            {
                printer.PrinterFirmwareVersion = match.Groups[1].Value;

            }
        }

        public void GetExtruderCount(string message, Printer printer)
        {
            var match = Regex.Match(message, @"EXTRUDER_COUNT:(\d+)");
            if (match.Success)
            {
                printer.NumberOfExtruders = int.Parse(match.Groups[1].Value);
            }
        }

        public void GetStepperDriverCurrents(string message, Printer printer)
        {
            var match = Regex.Match(message, @"M906 X(\d+\.?\d*) Y(\d+\.?\d*) Z(\d+\.?\d*)");
            if (match.Success)
            {
                printer.StepperDrivers.XDriverCurrent = double.Parse(match.Groups[1].Value);
                printer.StepperDrivers.YDriverCurrent = double.Parse(match.Groups[2].Value);
                printer.StepperDrivers.ZDriverCurrent = double.Parse(match.Groups[3].Value);
            }

            var matchE = Regex.Match(message, @"M906 T(\d+) E(\d+\.?\d*)");
            if (matchE.Success)
            {
                int extruderIndex = int.Parse(matchE.Groups[1].Value);
                double current = double.Parse(matchE.Groups[2].Value);

                switch (extruderIndex)
                {
                    case 0:
                        printer.StepperDrivers.EDriverCurrent = current;
                        break;
                    case 1:
                        printer.StepperDrivers.E2DriverCurrent = current;
                        break;
                    case 2:
                        printer.StepperDrivers.E3DriverCurrent = current;
                        break;
                    case 3:
                        printer.StepperDrivers.E4DriverCurrent = current;
                        break;
                    case 4:
                        printer.StepperDrivers.E5DriverCurrent = current;
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
                        printer.StepperDrivers.Z2DriverCurrent = current;
                        break;
                    case 2:
                        printer.StepperDrivers.Z3DriverCurrent = current;
                        break;
                    default:
                        Console.WriteLine("Invalid Z driver index");
                        break;
                }
            }
        }

        public void GetStepperDriverMode(string message, Printer printer)
        {
            var match = Regex.Match(message, @"(\w+)\s+driver mode:\s+(\w+)");

            if (match.Success)
            {
                var propertyMap = new Dictionary<string, Action<string>>
                    {
                        { "X", value => printer.StepperDrivers.XDriverSteppingMode = value },
                        { "Y", value => printer.StepperDrivers.YDriverSteppingMode = value },
                        { "Z", value => printer.StepperDrivers.ZDriverSteppingMode = value },
                        { "Z2", value => printer.StepperDrivers.Z2DriverSteppingMode = value },
                        { "Z3", value => printer.StepperDrivers.Z3DriverSteppingMode = value },
                        { "E", value => printer.StepperDrivers.EDriverSteppingMode = value },
                        { "E2", value => printer.StepperDrivers.E2DriverSteppingMode = value },
                    };

                string group1 = match.Groups[1].Value;
                string group2 = match.Groups[2].Value;

                if (propertyMap.ContainsKey(group1))
                {
                    if (group2 != null)
                        propertyMap[group1].Invoke(group2);
                }
            }
        }

        public void GetHomingSensitivityValues(string message, Printer printer)
        {
            var match = Regex.Match(message, @"M914 X(\d+\.?\d*) Y(\d+\.?\d*)?(?: Z(\d+\.?\d*))?");

            if (match.Success)
            {
                printer.StepperDrivers.XStallGuardTreshold = double.Parse(match.Groups[1].Value);

                if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    printer.StepperDrivers.YStallGuardTreshold = double.Parse(match.Groups[2].Value);
                }

                if (match.Groups[3].Success && !string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    printer.StepperDrivers.ZStallGuardTreshold = double.Parse(match.Groups[3].Value);
                }
            }
        }

        public void GetPrintJobStats(string input, Printer printer)
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
                    printer.PrintHistory.TotalPrints = int.Parse(printsMatch.Groups[1].Value);
                    printer.PrintHistory.TotalFinishedPrints = int.Parse(printsMatch.Groups[2].Value);
                    printer.PrintHistory.TotalFailedPrints = int.Parse(printsMatch.Groups[3].Value);
                }

                if (timeMatch.Success)
                {
                    printer.PrintHistory.TotalPrintTime = $"{timeMatch.Groups[1].Value}{timeMatch.Groups[2].Value}{timeMatch.Groups[3].Value}";
                    printer.PrintHistory.LongestPrintJob = $"{timeMatch.Groups[4].Value}{timeMatch.Groups[5].Value}";
                }

                if (filamentMatch.Success)
                {
                    printer.PrintHistory.FilamentUsed = filamentMatch.Groups[1].Value;
                }
            }
        }

        public void GetBinaryFileTransferState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"BINARY_FILE_TRANSFER:(\d+)");
            if (match.Success)
            {
                printer.HasBinaryFileTransfer = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoReportState(string message, Printer printer)
        {
            var matchPosition = Regex.Match(message, @"AUTOREPORT_POS:(\d+)");
            if (matchPosition.Success)
            {
                printer.HasAutoReportPosition = BoolOutput(int.Parse(matchPosition.Groups[1].Value));
            }

            var matchTemperature = Regex.Match(message, @"AUTOREPORT_TEMP:(\d+)");
            if (matchTemperature.Success)
            {
                printer.HasAutoReportTemperature = BoolOutput(int.Parse(matchTemperature.Groups[1].Value));
            }

            var matchSDStatus = Regex.Match(message, @"AUTOREPORT_SD_STATUS:(\d+)");
            if (matchSDStatus.Success)
            {
                printer.HasAutoReportSDStatus = BoolOutput(int.Parse(matchSDStatus.Groups[1].Value));
            }
        }

        public void GetEEPROMState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"EEPROM:(\d+)");
            if (match.Success)
            {
                printer.HasEEPROM = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetFilamentRunoutSensorState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"RUNOUT:(\d+)");
            if (match.Success)
            {
                printer.HasFilamentRunoutSensor = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetAutoLevelState(string message, Printer printer)
        {
            var hasAutoLevel = Regex.Match(message, @"AUTOLEVEL:(\d+)");
            var autoLevelingEnabled = Regex.Match(message, @"LEVELING:([A-Za-z]+)");

            if (autoLevelingEnabled.Success)
            {
                printer.AutoBedLevelingEnabled = autoLevelingEnabled.Groups[1].Value == "ON" ? true : autoLevelingEnabled.Groups[1].Value == "OFF" ? false : printer.AutoBedLevelingEnabled;

            }
            if (hasAutoLevel.Success)
            {
                printer.HasAutoBedLevel = BoolOutput(int.Parse(hasAutoLevel.Groups[1].Value));
            }
        }

        public void GetZProbeState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"Z_PROBE:(\d+)");
            if (match.Success)
            {
                printer.Head.ProbePresent = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSoftwarePowerState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"SOFTWARE_POWER:(\d+)");
            if (match.Success)
            {
                printer.HasSoftwarePowerControl = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetEmergencyParserState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"EMERGENCY_PARSER:(\d+)");
            if (match.Success)
            {
                printer.HasEmergencyParser = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetToggleLightsState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"TOGGLE_LIGHTS:(\d+)");
            if (match.Success)
            {
                printer.HasToggleLights = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetHostActionCommandsState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"HOST_ACTION_COMMANDS:(\d+)");
            if (match.Success)
            {
                printer.HasHostActionCommands = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPromptSupportState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"PROMPT_SUPPORT:(\d+)");
            if (match.Success)
            {
                printer.HasPromptSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetSDCardSupportState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"SDCARD:(\d+)");
            if (match.Success)
            {
                printer.HasSDCardSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetLongFilenameSupportState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"LONG_FILENAME:(\d+)");
            if (match.Success)
            {
                printer.HasLongFilenameSupport = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetCustomFirmwareUploadState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"CUSTOM_FIRMWARE_UPLOAD:(\d+)");
            if (match.Success)
            {
                printer.HasCustomFirmwareUpload = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetExtendedM20Support(string message, Printer printer)
        {
            var match = Regex.Match(message, @"EXTENDED_M20:(\d+)");
            if (match.Success)
            {
                printer.HasExtendedM20 = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetThermalProtectionState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"THERMAL_PROTECTION:(\d+)");
            if (match.Success)
            {
                printer.HasThermalProtection = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetBabyStepState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"BABYSTEPPING:(\d+)");
            if (match.Success)
            {
                printer.HasBabystep = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void GetPowerLossRecoveryState(string message, Printer printer)
        {
            var match = Regex.Match(message, @"M413 S(\d+)");
            if (match.Success)
            {
                printer.HasPowerLossRecovery = BoolOutput(int.Parse(match.Groups[1].Value));
            }
        }

        public void SetBedVolume(string input, Printer printer)
        {
            if (input != null)
            {
                var match = Regex.Match(input, _bED_VOLUME_PATTERN, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    printer.Bed.XSize = (int)double.Parse(match.Groups[1].Value);
                    printer.Bed.YSize = (int)double.Parse(match.Groups[2].Value);
                }
            }
        }

        public void SetMaximumFeedrates(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildMaxFeedrateCommand(printer.MotionSettings));
            }
        }

        public void SetStepsPerUnit(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetStepsPerUnitCommand(printer.MotionSettings));
            }
        }

        public void SetStartingAccelerations(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetStartingAccelerationCommand(printer.MotionSettings));
            }
        }

        public void SetMaximumAccelerations(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetMaxAccelerationCommand(printer.MotionSettings));
            }
        }

        public void SetAdvancedSettings(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetAdvancedSettingsCommand(printer.MotionSettings));
            }
        }

        public void SetOffsetSettings(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetHomeOffsetsCommand(printer.MotionSettings));
            }
        }


        public void SetBedLevelingOn(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                if (printer.AutoBedLevelingEnabled)
                {
                    printer.SerialConnection.Write("M420 S0");
                }
                else
                {
                    printer.SerialConnection.Write("M420 S1");
                }
            }
        }

        public void SetFadeHeight(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write($"M420 Z{printer.MotionSettings.FadeHeight}");
            }
        }

        public void SetPreheatingProfiles(Printer printer)
        {
            PreheatingProfiles.ForEach(profile =>
            {
                printer.SerialConnection.Write(CommandMethods.BuildMaterialPresetCommand(profile));
            });
        }

        public void SetDriverCurrents(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetDriverCurrentsCommand(printer.StepperDrivers));
            }
        }

        public void SetDriverSteppingMode(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetDriverSteppingMode(printer.StepperDrivers));
            }
        }

        public void SetBumpSensitivty(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetBumpSensitivity(printer.StepperDrivers));
            }
        }

        public void SetBedPidValues(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetBedPidValues(printer.BedTemperatures.PIDValues));
            }
        }

        public void SetHotendPidValues(Printer printer)
        {
            if (printer.SerialConnection.IsConnected)
            {
                printer.SerialConnection.Write(CommandMethods.BuildSetHotendPidValues(printer.HotendTemperatures.PIDValues));
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