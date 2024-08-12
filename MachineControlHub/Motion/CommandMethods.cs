using MachineControlHub.Gcode;
using MachineControlHub.Material;
using MachineControlHub.Temps;

namespace MachineControlHub.Motion
{

    /// <summary>
    /// Contains predefined methods for generating G-code commands as strings.
    /// This class simplifies the process of creating G-code commands for controlling a 3D printer.
    /// </summary>
    public class CommandMethods
    {
        const char G_PREFIX = 'G';
        const char M_PREFIX = 'M';



        /// <summary>
        /// Add a linear move to the queue to be performed after all previous moves are completed.
        /// </summary>
        /// <param name="position">Sets X,Y,Z,E position for the movement.</param>
        /// <param name="feedRate">Sets the speed for the movement.</param>
        /// <returns></returns>
        public static string BuildLinearMoveCommand(Position position, MotionSettingsData feedRate)
        {
            var linearMoveCommand = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.LinearMove,
                Parameters = new List<string>() 
                { 
                    position.XYZEMoveString(MovePositions.XMovePos), 
                    position.XYZEMoveString(MovePositions.YMovePos), 
                    position.XYZEMoveString(MovePositions.ZMovePos), 
                    position.XYZEMoveString(MovePositions.EMovePos), 
                    feedRate.FeedRateString(feedRate.FeedRateFreeMove) 
                }
            };
            return GCodeMethods.GCodeString(linearMoveCommand);
        }


        /// <summary>
        /// Builds a G-code command for relative positioning.
        /// </summary>
        /// <returns>A string representing the G-code command for relative positioning.</returns>
        public static string BuildRelativePositionCommand()
        {
            // Create a GCodeCommands object for relative positioning
            var relativePosition = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.RelativePositioning
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(relativePosition);
        }


        /// <summary>
        /// Builds a G-code command for absolute positioning.
        /// </summary>
        /// <returns>A string representing the G-code command for absolute positioning.</returns>
        public static string BuildAbsolutePositionCommand()
        {
            // Create a GCodeCommands object for absolute positioning
            var absolutePosition = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.AbsolutePositioning
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(absolutePosition);
        }


        /// <summary>
        /// Park the nozzle at a predefined XYZ position.
        /// Requires NOZZLE_PARK_FEATURE.
        /// The park position is defined by NOZZLE_PARK_POINT.
        /// </summary>
        /// <returns></returns>
        public static string BuildParkToolheadCommand()
        {
            var parkToolhead = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.ParkToolHead,
                Parameters = new List<string>() { "P2" }
            };

            return GCodeMethods.GCodeString(parkToolhead);
        }


        /// <summary>
        /// The G28 command is used to home one or more axes. The default behavior with no parameters is to home all axes.
        /// If one or more axes are added as parameters it will home only the axes added.
        /// </summary>
        /// <param name="x">true to home X axis</param>
        /// <param name="y">true to home Y axis</param>
        /// <param name="z">true to home Z axis</param>
        /// <returns></returns>
        public static string BuildHomeAxesCommand(bool x = false, bool y = false, bool z = false)
        {
            string XYZ = "";

            if (x)
            {
                XYZ += "X ";
            }

            if (y)
            {
                XYZ += "Y ";
            }

            if (z)
            {
                XYZ += "Z";
            }

            var homeAxes = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.HomeAllAxes,
                Parameters = new List<string>() { XYZ }
            };

            return GCodeMethods.GCodeString(homeAxes);
        }


        /// <summary>
        /// It levels the bed regarding the bed leveling system in the firmware.
        /// </summary>
        /// <returns></returns>
        public static string BuildBedLevelingCommand()
        {
            var bedLeveling = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.AutoBedLeveling
            };

            return GCodeMethods.GCodeString(bedLeveling);
        }


        /// <summary>
        /// This command can be used to disable one or more steppers (X,Y,Z,E).
        /// </summary>
        /// <returns></returns>
        public static string BuildDisableSteppersCommand()
        {
            var disableSteppers = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.DisableSteppers,
            };

            return GCodeMethods.GCodeString(disableSteppers);
        }


        /// <summary>
        /// List all printable files on the SD card back to the requesting serial port in compact DOS 8.3 format.
        /// Only files with .gcode, .gco, and .g extensions will be listed.
        /// Hidden files (beginning with .) will not be listed.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildListSDCardCommand()
        {
            var sdCard = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ListSdCard,
                Parameters = new List<string>() { "L T" }
            };

            return GCodeMethods.GCodeString(sdCard);
        }


        /// <summary>
        /// Use this command to mount the last-selected SD card or thumb drive.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildInitSDCardCommand()
        {
            var sdCard = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.InitSdCard
            };

            return GCodeMethods.GCodeString(sdCard);
        }


        /// <summary>
        /// Select an SD file for printing or processing.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName">path to the gcode file on SD card</param>
        /// <returns></returns>
        public static string BuildSelectSDFileCommand(string fileName)
        {
            var selectSDCard = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SelectSdFile
            };

            return GCodeMethods.GCodeString(selectSDCard, fileName);
        }


        public static string BuildReportSDStatus(int interval)
        {
            var reportSDStatus = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ReportSdPrintStatus,
                Parameters = new List<string>() { $"S{interval}" }
            };

            return GCodeMethods.GCodeString(reportSDStatus);
        }


        /// <summary>
        /// Start an SD print.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildStartSDPrintCommand()
        {
            var startSDPrint = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.StartPrint,

            };

            return GCodeMethods.GCodeString(startSDPrint);
        }


        /// <summary>
        /// Pause the SD print in progress. If PARK_HEAD_ON_PAUSE is enabled, park the nozzle.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildPauseSDPrintCommand()
        {
            var pauseSDPrint = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.PauseSdPrint,
            };

            return GCodeMethods.GCodeString(pauseSDPrint);
        }


        /// <summary>
        /// Pause the SD print in progress. If PARK_HEAD_ON_PAUSE is enabled, park the nozzle.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildStopSDPrintCommand()
        {
            var stopSDPrint = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.AbortSDPrint,
            };

            return GCodeMethods.GCodeString(stopSDPrint);
        }


        /// <summary>
        /// This command starts a file write. All commands received by Marlin are written to the file and are not executed until M29 closes the file.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName">path to the gcode file on SD card</param>
        /// <returns></returns>
        public static string BuildStartSDWriteCommand(string fileName)
        {
            var startSDWrite = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.StartSdWrite,
            };

            return GCodeMethods.GCodeString(startSDWrite, fileName);
        }


        /// <summary>
        /// Stop writing to a file that was begun with M28.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string BuildStopSDWriteCommand()
        {
            var stopSDWrite = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.StopSdWrite
            };

            return GCodeMethods.GCodeString(stopSDWrite);
        }


        /// <summary>
        /// Builds a G-code command to retrieve the current position of the 3D printer.
        /// </summary>
        /// <returns>A string representing the G-code command to get the current position.</returns>
        public static string BuildGetCurrentPositionCommand()
        {
            // Create a GCodeCommands object for retrieving the current position
            var currentPosition = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.GetCurrentPosition
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(currentPosition);
        }


        /// <summary>
        /// Builds a G-code command to set the LCD status with a custom message.
        /// </summary>
        /// <param name="message">The custom message to be displayed on the LCD.</param>
        /// <returns>A string representing the G-code command to set the LCD status.</returns>
        public static string BuildSetLCDStatusCommand(string message)
        {
            // Create a GCodeCommands object for setting LCD status
            var setLCDStatus = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetLCDStatus,
            };

            // Convert the GCodeCommands object to a G-code string, including the custom message
            return GCodeMethods.GCodeString(setLCDStatus, message);
        }


        /// <summary>
        /// Delete a file from the SD card.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName">path to the gcode file on SD card</param>
        /// <returns></returns>
        public static string BuildDeleteSDFileCommand(string fileName)
        {
            var deleteSDFile = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.DeleteSdFile
            };

            return GCodeMethods.GCodeString(deleteSDFile, fileName);
        }


        /// <summary>
        /// Set a new target hot end temperature and continue without waiting.
        /// The firmware will continue to try to reach and hold the temperature in the background.
        /// </summary>
        /// <param name="setTemp"> Accepts SetHotendTemperature field from HotendTemps</param>
        /// <returns></returns>
        public static string BuildSetHotendTempCommand(int setTemp)
        {
            var hotendTemp = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetHotendTemperature,
            };

            return GCodeMethods.GCodeString(hotendTemp, setTemp);
        }


        /// <summary>
        /// Set a new target temperature for the heated bed and continue without waiting.
        /// The firmware manages heating in the background.
        /// </summary>
        /// <param name="setTemp"></param>
        /// <returns></returns>
        public static string BuildSetBedTempCommand(int setTemp)
        {
            var bedTemp = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetBedTemperature
            };

            return GCodeMethods.GCodeString(bedTemp, setTemp);
        }


        /// <summary>
        /// Set a new target heated chamber temperature and continue without waiting.
        /// The firmware will continue to try to reach and hold the temperature in the background.
        /// </summary>
        /// <param name="setTemp"></param>
        /// <returns></returns>
        public static string BuildSetChamberTempCommand(int setTemp)
        {
            var chamberTemp = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetChamberTemperature
            };

            return GCodeMethods.GCodeString(chamberTemp, setTemp);
        }


        /// <summary>
        /// Builds a G-code command to request a report of temperatures from the 3D printer.
        /// </summary>
        /// <returns>A string representing the G-code command to request temperature reports.</returns>
        public static string BuildReportTemperaturesCommand()
        {
            // Create a GCodeCommands object for reporting temperatures
            var tempReport = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ReportTemperatures
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(tempReport);
        }

        public static string BuildAutoReportTemperaturesCommand(int interval)
        {
            var tempReport = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.TemperatureAutoReport,
                Parameters = new List<string>() { $"S{interval}" }
            };

            return GCodeMethods.GCodeString(tempReport);
        }

        /// <summary>
        /// Turn on one of the fans and set its speed. If no fan index is given, the print cooling fan is selected.
        /// </summary>
        /// <param name="fanSpeed"></param>
        /// <returns></returns>
        public static string BuildFanSpeedCommand(int fanSpeed)
        {
            var setFanSpeed = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFanSpeed
            };

            return GCodeMethods.GCodeString(setFanSpeed, fanSpeed);
        }


        /// <summary>
        /// Turn off one of the fans. If no fan index is given, the print cooling fan.
        /// </summary>
        /// <returns></returns>
        public static string BuildFanOffCommand()
        {
            var fanOff = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.FanOff
            };

            return GCodeMethods.GCodeString(fanOff);
        }


        /// <summary>
        /// Set the preheating presets for materials in the LCD menu.
        /// </summary>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public static string BuildMaterialPresetCommand(PreheatingProfiles profiles)
        {
            var preset = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetMaterialPreset,
                Parameters = new List<string>() 
                { 
                    profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.MaterialIndex, profiles.MaterialIndex), 
                    profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Hotend, profiles.HotendTemp), 
                    profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Bed, profiles.BedTemp), 
                    profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Fan, profiles.FanSpeed) 
                }
            };

            return GCodeMethods.GCodeString(preset);
        }

        public static string BuildPIDAutoTuneCommand(int index, int temp, int cycles)
        {
            var pidAutoTune = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.PIDAutoTune,
                Parameters = new List<string>() { $"E{index} C{cycles} S{temp}" }
            };

            return GCodeMethods.GCodeString(pidAutoTune);
        }


        /// <summary>
        /// Set the filament’s current diameter
        /// diameter refers to FilamentProperties.FilamentDiameter
        /// </summary>
        /// <returns></returns>
        public static string BuildFilamentDiameterCommand(double diamaeter)
        {
            var filamentDiameter = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFilamentDiameter,
                Parameters = new List<string>() { $"D{diamaeter}" }
            };

            return GCodeMethods.GCodeString(filamentDiameter);
        }


        /// <summary>
        /// Set the max feedrate for one or more axes (in current units-per-second).
        /// </summary>
        /// <param name="motion"></param>
        /// <returns></returns>
        public static string BuildMaxFeedrateCommand(MotionSettingsData motion)
        {
            var maxFeedrate = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetMaxFeedrates,
                Parameters = new List<string>() 
                {
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.E, motion.EMaxFeedrate),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.X, motion.XMaxFeedrate),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Y, motion.YMaxFeedrate),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Z, motion.ZMaxFeedrate) 
                }
            };

            return GCodeMethods.GCodeString(maxFeedrate);
        }

        public static string BuildSaveToEEPROMCommand()
        {
            var saveToEEPROM = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SaveSettings
            };

            return GCodeMethods.GCodeString(saveToEEPROM);
        }


        /// <summary>
        /// Apply a persistent offset to the native home position and coordinate space.
        /// This effectively shifts the coordinate space in the negative direction.
        /// </summary>
        /// <param name="motion"></param>
        /// <returns></returns>
        public static string BuildHomeOffsetsCommand(MotionSettingsData motion)
        {
            var homeOffsets = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetHomeOffsets,
                Parameters = new List<string>() 
                {
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.X, motion.XHomeOffset),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Y, motion.YHomeOffset),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Z, motion.ZHomeOffset) 
                }
            };

            return GCodeMethods.GCodeString(homeOffsets);
        }


        /// <summary>
        /// Builds a G-code command to request a report of current settings from the 3D printer.
        /// </summary>
        /// <returns>A string representing the G-code command to request settings reports.</returns>
        public static string BuildReportSettings()
        {
            // Create a GCodeCommands object for reporting settings
            var settings = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ReportSettings
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(settings);
        }


        /// <summary>
        /// Builds a G-code command to initiate a filament change on the 3D printer.
        /// </summary>
        /// <returns>A string representing the G-code command to initiate filament change.</returns>
        public static string BuildFilamentChangeCommand()
        {
            // Create a GCodeCommands object for filament change
            var filamentChange = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.FilamentChange
            };

            // Convert the GCodeCommands object to a G-code string
            return GCodeMethods.GCodeString(filamentChange);
        }

        public static string BuildLoadFilamentCommand()
        {
            var loadFilament = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.LoadFilament
            };

            return GCodeMethods.GCodeString(loadFilament);
        }

        public static string BuildUnloadFilamentCommand()
        {
            var unloadFilament = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.UnloadFilament
            };

            return GCodeMethods.GCodeString(unloadFilament);
        }

        public static string BuildPrintProgressCommand()
        {
            var printTime = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetPrintProgress
            };

            return GCodeMethods.GCodeString(printTime);
        }

        public static string BuildSetStepsPerUnitCommand(MotionSettingsData motion)
        {
            var stepsPerUnit = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetAxisStepsPerUnit,
                Parameters = new List<string>() 
                {
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.E, motion.EStepsPerUnit),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.X, motion.XStepsPerUnit),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Y, motion.YStepsPerUnit),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Z, motion.ZStepsPerUnit) 
                }

            };

            return GCodeMethods.GCodeString(stepsPerUnit);

        }

        public static string BuildSetStartingAccelerationCommand(MotionSettingsData motion)
        {
            var startingAcceleration = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetStartingAcceleration,
                Parameters = new List<string>() { $"P{motion.PrintAcceleration} ", $"R{motion.RetractAcceleration} " , $"T{motion.TravelAcceleration} " }
            };

            return GCodeMethods.GCodeString(startingAcceleration);
        }

        public static string BuildSetMaxAccelerationCommand(MotionSettingsData motion)
        {
            var maxAcceleration = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.PrintTravelMoveLimits,
                Parameters = new List<string>() 
                {
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.E, motion.EMaxAcceleration), $"F{motion.PlannerFrequencyLimit} ", $"S{motion.PlannerXYFrequencyMinimumSpeedPercentage} ",$"T{motion.TargetExtruder} ",
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.X, motion.XMaxAcceleration), MotionSettingsData.AxisString(MotionSettingsData.Axis.Y, motion.YMaxAcceleration),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Z, motion.ZMaxAcceleration) }
            };

            return GCodeMethods.GCodeString(maxAcceleration);
        }

        public static string BuildSetAdvancedSettingsCommand(MotionSettingsData motion)
        {
            var advancedSettings = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetAdvancedSettings,
                Parameters = new List<string>() 
                { 
                    $"B{motion.MinSegmentTime} ", 
                    $"E{motion.EMaxJerk} ", 
                    $"J{motion.JunctionDeviation} ",
                    $"S{motion.MinPrintFeedrate} ",
                    $"T{motion.MinTravelFeedrate} ",
                    $"X{motion.XMaxJerk} ",
                    $"Y{motion.YMaxJerk} ",
                    $"Z{motion.ZMaxJerk} " }
            };

            return GCodeMethods.GCodeString(advancedSettings);
        }

        public static string BuildSetHomeOffsetsCommand(MotionSettingsData motion)
        {
            var homeOffsets = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetHomeOffsets,
                Parameters = new List<string>() 
                {
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.X, motion.XHomeOffset),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Y, motion.YHomeOffset),
                    MotionSettingsData.AxisString(MotionSettingsData.Axis.Z, motion.ZHomeOffset) }
            };

            return GCodeMethods.GCodeString(homeOffsets);
        }

        public static string BuildHostKeepAliveCommand(int interval)
        {
            var hostKeepAlive = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.HostKeepAlive,
                Parameters = new List<string>() { $"S{interval}" }
            };

            return GCodeMethods.GCodeString(hostKeepAlive);
        }

        public static string BuildSetSoftwareEndstopsCommand(bool enable)
        {
            var softwareEndstops = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SoftwareEndstops,
                Parameters = new List<string>() { enable ? "S1" : "S0" }
            };

            return GCodeMethods.GCodeString(softwareEndstops);
        }

        public static string BuildCheckSoftwareEndstopsCommand()
        {
            var checkSoftwareEndstops = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SoftwareEndstops
            };

            return GCodeMethods.GCodeString(checkSoftwareEndstops);
        }

        public static string BuildRequestPrintFlowCommand()
        {
            var requestPrintSpeed = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFlowPercentage
            };

            return GCodeMethods.GCodeString(requestPrintSpeed);
        }

        public static string BuildSetPrintFlowCommand(int flowrate)
        {
            var setFlowRate = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFlowPercentage,
                Parameters = new List<string>() { $"S{flowrate} " }
            };

            return GCodeMethods.GCodeString(setFlowRate);

        }

        public static string BuildRequestPrintSpeedCommand()
        {
            var requestPrintSpeed = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFeedratePercentage
            };

            return GCodeMethods.GCodeString(requestPrintSpeed);
        }

        public static string BuildSetPrintSpeedCommand(int feedrate)
        {
            var setPrintSpeed = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetFeedratePercentage,
                Parameters = new List<string>() { $"S{ feedrate } " }
            };

            return GCodeMethods.GCodeString(setPrintSpeed);
        }

        public static string BuildRequestCurrentPositionsCommand()
        {
            var requestCurrentPositions = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.GetCurrentPosition
            };

            return GCodeMethods.GCodeString(requestCurrentPositions);
        }

        public static string BuildBabySteppingCommand(double value)
        {
            var babyStepping = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.BabyStep,
                Parameters = new List<string>() { $"Z{value}" }
            };

            return GCodeMethods.GCodeString(babyStepping);
        }

        public static string BuildAdjustDualZMotorCommand()
        {
            var adjustDualZMotor = new GCodeCommands
            {
                Type = G_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.GCommands.ZSteppersAutoAlignment
            };

            return GCodeMethods.GCodeString(adjustDualZMotor);
        }

        public static string BuildSetDriverCurrentsCommand(StepperDriversData driver)
        {
            var setDriverCurrents = new GCodeCommands
            {
                Parameters = new List<string>()
            }; 
            
            var driverCurrents = driver.GetDriverCurrents();

            foreach (var kvp in driverCurrents)
            {
                if (kvp.Value.HasValue)
                {
                    setDriverCurrents.Parameters.Add($"\n M906 {kvp.Key} {kvp.Value.Value}");
                }
            }

            return GCodeMethods.GCodeString(setDriverCurrents);
        }

        public static string BuildSetDriverSteppingMode(StepperDriversData driver)
        {
            var setDriverSteppingMode = new GCodeCommands
            {
                Parameters = new List<string>()
            };

            var driverSteppingMode = driver.GetDriverSteppingMode();

            foreach (var kvp in driverSteppingMode)
            {
                if(kvp.Value != string.Empty)
                {
                    if (kvp.Value == "StealthChop")
                    {
                        setDriverSteppingMode.Parameters.Add($"\n M569 S1 {kvp.Key}");
                    }
                    if (kvp.Value == "SpreadCycle")
                    {
                        setDriverSteppingMode.Parameters.Add($"\n M569 S0 {kvp.Key}");
                    }
                }
            }

            return GCodeMethods.GCodeString(setDriverSteppingMode);
        }

        public static string BuildRequestPrintJobStatsCommand()
        {

           var requestPrintJobStats = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.PrintJobStats
            };

            return GCodeMethods.GCodeString(requestPrintJobStats);
        }

        public static string BuildSetBumpSensitivity(StepperDriversData driver)
        {
            var setBumpSensitivity = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.TMCBumpSensitivity,
                Parameters = new List<string>() { $"X{driver.XStallGuardTreshold} Y{driver.YStallGuardTreshold} Z{driver.ZStallGuardTreshold}" }
            };

            return GCodeMethods.GCodeString(setBumpSensitivity);
        }

        public static string BuildSetBedPidValues(PIDValues bed)
        {
            var setBedPidValues = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetBedPID,
                Parameters = new List<string>() { $"P{bed.Proportional} I{bed.Integral} D{bed.Derivative}" }
            };

            return GCodeMethods.GCodeString(setBedPidValues);
        }

        public static string BuildSetHotendPidValues(PIDValues hotend)
        {
            var setHotendPidValues = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetHotendPID,
                Parameters = new List<string>() { $"P{hotend.Proportional} I{hotend.Integral} D{hotend.Derivative}" }
            };

            return GCodeMethods.GCodeString(setHotendPidValues);
        }
    }
}