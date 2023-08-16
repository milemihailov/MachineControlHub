using ControllingAndManagingApp.Gcode;

namespace ControllingAndManagingApp.Motion
{
    public class CommandMethods
    {


        /// <summary>
        /// Add a linear move to the queue to be performed after all previous moves are completed.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedRate"></param>
        /// <returns></returns>
        public static string LinearMove(Position position, MotionSettingsData feedRate)
        {
            var linearMoveCommand = new GCodeCommandsData
            {
                Type = 'G',
                Instruction = (int)GCodeInstructionsEnums.GCommands.LinearMove,
                Parameters = new List<string>() { $"{position.XYZEMoveString(MovePositions.XMovePos)}", $"{position.XYZEMoveString(MovePositions.YMovePos)}", $"{position.XYZEMoveString(MovePositions.ZMovePos)}", $"{position.XYZEMoveString(MovePositions.EMovePos)}", $"{feedRate.FeedRateString()}" }
            };
            return GCodeMethods.GCodeString(linearMoveCommand);

        }


        /// <summary>
        /// Park the nozzle at a predefined XYZ position.
        /// Requires NOZZLE_PARK_FEATURE.
        /// The park position is defined by NOZZLE_PARK_POINT.
        /// </summary>
        /// <returns></returns>
        public static string ParkToolhead()
        {
            var parkToolhead = new GCodeCommandsData
            {
                Type = 'G',
                Instruction = (int)GCodeInstructionsEnums.GCommands.ParkToolHead,
                Parameters = new List<string>() { "P2" }
            };

            return GCodeMethods.GCodeString(parkToolhead);
        }


        /// <summary>
        /// The G28 command is used to home one or more axes. The default behavior with no parameters is to home all axes.
        /// </summary>
        /// <returns></returns>
        public static string HomeAxes(Position position)
        {
            var homeAxes = new GCodeCommandsData
            {
                Type = 'G',
                Instruction = (int)GCodeInstructionsEnums.GCommands.HomeAllAxes,
                Parameters = new List<string>() { $"{position.XYZHomeString(HomePositions.XHomePos)}", $"{position.XYZHomeString(HomePositions.YHomePos)}", $"{position.XYZHomeString(HomePositions.ZHomePos)}" }
            };

            return GCodeMethods.GCodeString(homeAxes);
        }


        /// <summary>
        /// It levels the bed regarding the bed leveling option chosen in the firmware.
        /// </summary>
        /// <returns></returns>
        public static string BedLeveling()
        {
            var bedLeveling = new GCodeCommandsData
            {
                Type = 'G',
                Instruction = (int)GCodeInstructionsEnums.GCommands.AutoBedLeveling
            };

            return GCodeMethods.GCodeString(bedLeveling);
        }


        /// <summary>
        /// This command can be used to disable one or more steppers (X,Y,Z,E).
        /// </summary>
        /// <returns></returns>
        public static string DisableSteppers()
        {
            var disableSteppers = new GCodeCommandsData
            {
                Type = 'M',
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
        public static string ListSDCard()
        {
            var sdCard = new GCodeCommandsData
            {
                Type = 'M',
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
        public static string InitSDCard()
        {
            var sdCard = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.InitSdCard
            };

            return GCodeMethods.GCodeString(sdCard);
        }


        /// <summary>
        /// Select an SD file for printing or processing.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string SelectSDCard(string fileName)
        {
            var selectSDCard = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.SelectSdFile
            };

            return GCodeMethods.GCodeString(selectSDCard, fileName);
        }


        /// <summary>
        /// Start an SD print.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string StartSDPrint()
        {
            var startSDPrint = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.StartPrint,

            };
            return GCodeMethods.GCodeString(startSDPrint);
        }


        /// <summary>
        /// Pause the SD print in progress. If PARK_HEAD_ON_PAUSE is enabled, park the nozzle.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string PauseSDPrint()
        {
            var pauseSDPrint = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.PauseSdPrint,
            };
            return GCodeMethods.GCodeString(pauseSDPrint);
        }


        /// <summary>
        /// This command starts a file write. All commands received by Marlin are written to the file and are not executed until M29 closes the file.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string StartSDWrite(string fileName)
        {
            var startSDWrite = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.StartSdWrite,
            };
            return GCodeMethods.GCodeString(startSDWrite, fileName);
        }


        /// <summary>
        /// Stop writing to a file that was begun with M28.
        /// Requires SDSUPPORT
        /// </summary>
        /// <returns></returns>
        public static string StopSDWrite()
        {
            var stopSDWrite = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.StopSdWrite
            };
            return GCodeMethods.GCodeString(stopSDWrite);
        }


        /// <summary>
        /// Delete a file from the SD card.
        /// Requires SDSUPPORT
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string DeleteSDFile(string fileName)
        {
            var deleteSDFile = new GCodeCommandsData
            {
                Type = 'M',
                Instruction = (int)GCodeInstructionsEnums.MCommands.DeleteSdFile
            };
            return GCodeMethods.GCodeString(deleteSDFile, fileName);
        }
    }
}