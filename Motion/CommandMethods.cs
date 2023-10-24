using ControllingAndManagingApp.Gcode;
using ControllingAndManagingApp.Material;
using ControllingAndManagingApp.SerialConnection;

namespace ControllingAndManagingApp.Motion
{

    /// <summary>
    /// Contains predefined methods for generating G-code commands as strings.
    /// This class simplifies the process of creating G-code commands for controlling a 3D printer.
    /// </summary>
    public class CommandMethods
    {
        const string GCODE_FILE_EXTENSION = ".gco";
        const string PRINT_ABORT_MESSAGE = "Print Aborted";
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
        public static string SendHomeAxes(bool x = false, bool y = false, bool z = false)
        {
            string XYZ = "";

            if (x)
            {
                XYZ += "X";
            }

            if (y)
            {
                XYZ += "Y";
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


        public static string BuildGetCurrentPositionCommand()
        {
            var currentPosition = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.GetCurrentPosition
            };

            return GCodeMethods.GCodeString(currentPosition);
        }


        public static string BuildSetLCDStatusCommand(string message)
        {
            var setLCDStatus = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.SetLCDStatus,
            };

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


        public static string BuildReportTemperaturesCommand()
        {
            var tempReport = new GCodeCommands
            {
                Type = M_PREFIX,
                Instruction = (int)GCodeInstructionsEnums.MCommands.ReportTemperatures
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
                Parameters = new List<string>() { profiles.MaterialIndexString(), profiles.HotendTempString(), profiles.BedTempString(), profiles.FanSpeedString() }
            };

            return GCodeMethods.GCodeString(preset);
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
                Parameters = new List<string>() { motion.EString(motion.EMaxFeedrate), motion.XString(motion.XMaxFeedrate), motion.YString(motion.YMaxFeedrate), motion.ZString(motion.ZMaxFeedrate) }
            };

            return GCodeMethods.GCodeString(maxFeedrate);
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
                Parameters = new List<string>() { motion.XString(motion.XHomeOffset), motion.YString(motion.YHomeOffset), motion.ZString(motion.ZHomeOffset) }
            };

            return GCodeMethods.GCodeString(homeOffsets);
        }


        /// <summary>
        /// Transfers a G-code file to the SD card using a serial connection.
        /// </summary>
        /// <param name="GcodeFilePath">The path to the G-code file to be transferred.</param>
        /// <param name="fileName">The name to assign to the file on the SD card.</param>
        /// <remarks>
        /// This method sends the G-code commands line by line to the printer's SD card via the serial connection.
        /// It starts by writing the file name and then sends the contents of the file, followed by a stop command.
        /// </remarks>
        /// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
        /// <exception cref="Exception">Thrown for any other errors that occur during the file transfer.</exception>
        public static void TransferFileToSD(string GcodeFilePath, string fileName, SerialInterface serial)
        {
            try
            {
                using (StreamReader reader = new StreamReader(GcodeFilePath))
                {
                    string line;
                    serial.Write($"{BuildStartSDWriteCommand(fileName)}{GCODE_FILE_EXTENSION}");

                    while ((line = reader.ReadLine()) != null)
                    {
                        serial.Write(line);
                        Console.WriteLine(line);
                    }

                    serial.Write(BuildStopSDWriteCommand());
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }


        /// <summary>
        /// Aborts the current print by sending necessary commands to the provided serial interface.
        /// </summary>
        /// <param name="serial">The serial interface used for communication.</param>
        public static void AbortCurrentPrint(SerialInterface serial)
        {
            // Send commands to stop the SD print and set the LCD status.
            serial.Write(BuildStopSDPrintCommand());
            serial.Write(BuildSetLCDStatusCommand(PRINT_ABORT_MESSAGE));

            // Print a message to the console.
            Console.WriteLine(PRINT_ABORT_MESSAGE);
        }

    }
}