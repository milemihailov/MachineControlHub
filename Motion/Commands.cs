using ControllingAndManagingApp.Gcode;

namespace ControllingAndManagingApp.Motion
{
    public class Commands
    {
        public static string RapidLinearMove(Position position, MotionSettingsData feedRate)
        {
            var moveCommand = new GcodeCommand();
            moveCommand.Type = 'G';
            moveCommand.Instruction = (int)GCodeInstructions.GCommands.RapidLinearMove;
            moveCommand.Parameters = new List<string>() { $"{position.XYZEMoveString(MovePositions.XMovePos)}", $"{position.XYZEMoveString(MovePositions.YMovePos)}", $"{position.XYZEMoveString(MovePositions.ZMovePos)}", $"{position.XYZEMoveString(MovePositions.EMovePos)}", $"{feedRate.FeedRateString()}" };

            return moveCommand.GcodeLine();
        }

        /// <summary>
        /// Homes all axes if no parameters are used.
        /// If specified homes only the specified axes.
        /// </summary>
        /// <returns></returns>
        public static string HomeAxes(Position position)
        {
            var homeAxes = new GcodeCommand();
            homeAxes.Type = 'G';
            homeAxes.Instruction = (int)GCodeInstructions.GCommands.HomeAllAxes;
            homeAxes.Parameters = new List<string>() { $"{position.XYZHomeString(HomePositions.XHomePos)}", $"{position.XYZHomeString(HomePositions.YHomePos)}", $"{position.XYZHomeString(HomePositions.ZHomePos)}" };

            return homeAxes.GcodeLine();
        }

        /// <summary>
        /// Disables all steppers if no parameters are used.
        /// If specified disables only the specified stepper.
        /// </summary>
        /// <returns></returns>
        public static string DisableSteppers()
        {
            var disableSteppers = new GcodeCommand
            {
                Type = 'M',
                Instruction = (int)GCodeInstructions.MCommands.DisableSteppers,
            };

            return disableSteppers.GcodeLine();
        }
    }
}