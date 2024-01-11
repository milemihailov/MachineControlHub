using MachineControlHub.Motion;

namespace WebUI.Data
{


    public class ControlPanelService
    {

        public MotionSettingsData feedRate;

        public ControlPanelService()
        {
            feedRate = new MotionSettingsData();
        }

        public const int MAX_FAN_SPEED = 255;
        public const double AXIS_MOVEMENT_BY_0_1 = 0.1;
        public const double AXIS_MOVEMENT_BY_1 = 1;
        public const double AXIS_MOVEMENT_BY_10 = 10;
        public const double AXIS_MOVEMENT_BY_100 = 100;

        public int defaultFanSpeed;
        public string sendCommand = "";

        double valueToMove = AXIS_MOVEMENT_BY_10;
        /// <summary>
        /// Updates the movement value used for incremental movements.
        /// </summary>
        /// <param name="newValue">The new value for the movement. Should be a positive numeric value.</param>
        public void UpdateIncrementalMovementValue(double newValue)
        {
            // Set the new value for the movement
            valueToMove = newValue;
        }



        /// <summary>
        /// Adjusts the specified axis position by the given value.
        /// </summary>
        /// <param name="position">The axis to adjust.</param>
        /// <param name="increment">True to increment, false to decrement.</param>
        public void AdjustAxis(MovePositions position, bool increment, ConnectionServiceSerial serial)
        {
            // Create a new Position object to store the movement values
            Position pos = new();

            // Send a command to set relative positioning
            serial.Write(CommandMethods.BuildRelativePositionCommand());

            // Determine the movement value based on whether it's an increment or decrement
            double moveValue = increment ? valueToMove : -valueToMove;

            // Set the appropriate property in the Position object based on the specified axis
            switch (position)
            {
                case MovePositions.XMovePos:
                    pos.XMovePosition = moveValue;
                    break;
                case MovePositions.YMovePos:
                    pos.YMovePosition = moveValue;
                    break;
                case MovePositions.ZMovePos:
                    pos.ZMovePosition = moveValue;
                    break;
                case MovePositions.EMovePos:
                    pos.EMovePosition = moveValue;
                    break;
            }

            // Send a command to perform a linear move with the specified values and feed rate
            serial.Write(CommandMethods.BuildLinearMoveCommand(pos,feedRate));
            serial.Write(CommandMethods.BuildAbsolutePositionCommand());
        }

        /// <summary>
        /// Sends a Gcode command via the terminal connection.
        /// </summary>
        /// <param name="command">The Gcode command to send.</param>
        /// <param name="serial">The serial connection service.</param>
        /// <remarks>
        /// The provided Gcode command will be sent to the terminal connection after converting it to lowercase.
        /// </remarks>
        public void SendGcodeViaTerminal(string command, ConnectionServiceSerial serial)
        {
            // Implementation details
            serial.Write(command.ToLower());
        }

        public void SetFanSpeed(int value)
        {
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFanSpeedCommand(value));
        }

        public void SetFanOff()
        {
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFanOffCommand());
        }

        public void HomeAxisCommand(bool x = false, bool y = false, bool z = false)
        {
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildHomeAxesCommand(x,y,z));
        }

        public void DisableSteppers()
        {
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildDisableSteppersCommand());
        }
    }
}
