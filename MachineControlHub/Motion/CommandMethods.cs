using MachineControlHub.Gcode;
using MachineControlHub.Material;

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
                Parameters = new List<string>() { position.XYZEMoveString(MovePositions.XMovePos), position.XYZEMoveString(MovePositions.YMovePos), position.XYZEMoveString(MovePositions.ZMovePos), position.XYZEMoveString(MovePositions.EMovePos), feedRate.FeedRateString(feedRate.FeedRateFreeMove) }
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


        public static string BuildReportSDStatus()
        {
            var reportSDStatus = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ReportSdPrintStatus
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
                Parameters = new List<string>() { profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.MaterialIndex, profiles.MaterialIndex), profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Hotend, profiles.HotendTemp), profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Bed, profiles.BedTemp), profiles.StringWithPrefixAndValue(PreheatingProfiles.Prefixes.Fan, profiles.FanSpeed) }
            };

            return GCodeMethods.GCodeString(preset);
        }

        public static string BuildPIDAutoTuneCommand(int index, int temp, int cycles, bool useCycle)
        {
            var pidAutoTune = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.PIDAutoTune,
                Parameters = new List<string>() { $"E{index} C{cycles} S{temp} U={useCycle}" }
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
                Parameters = new List<string>() { motion.AxisString(MotionSettingsData.Axis.E, motion.EMaxFeedrate), motion.AxisString(MotionSettingsData.Axis.X, motion.XMaxFeedrate), motion.AxisString(MotionSettingsData.Axis.Y, motion.YMaxFeedrate), motion.AxisString(MotionSettingsData.Axis.Z, motion.ZMaxFeedrate) }
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
                Parameters = new List<string>() { motion.AxisString(MotionSettingsData.Axis.X, motion.XHomeOffset), motion.AxisString(MotionSettingsData.Axis.Y, motion.YHomeOffset), motion.AxisString(MotionSettingsData.Axis.Z, motion.ZHomeOffset) }
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
                Parameters = new List<string>() { motion.AxisString(MotionSettingsData.Axis.E, motion.EStepsPerUnit), motion.AxisString(MotionSettingsData.Axis.X, motion.XStepsPerUnit), motion.AxisString(MotionSettingsData.Axis.Y, motion.YStepsPerUnit), motion.AxisString(MotionSettingsData.Axis.Z, motion.ZStepsPerUnit) }

            };

            return GCodeMethods.GCodeString(stepsPerUnit);

        }

    }
}