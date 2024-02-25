using MachineControlHub.Motion;
using System.Text.RegularExpressions;

namespace WebUI.Data
{
    public class ControlPanelService
    {
        public const string FAN_PATTERN = @"M106 P0 S(\d+)";

        public string consoleOutput;
        public int defaultFanSpeed;
        public double fanSpeedInPercentage;
        public string sendCommand = "";
        public double valueToMove = 10;

        public MotionSettingsData feedRate;

        public ControlPanelService()
        {
            feedRate = new MotionSettingsData();
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
            serial.Write(CommandMethods.BuildLinearMoveCommand(pos, feedRate));
            serial.Write(CommandMethods.BuildAbsolutePositionCommand());
        }


        /// <summary>
        /// Sends a command to disable the steppers of the 3D printer.
        /// </summary>
        public void DisableSteppers()
        {   
            // Send the command to disable the steppers to the printer
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildDisableSteppersCommand());
        }


        /// <summary>
        /// Sends a command to home one or more axes of the 3D printer.
        /// </summary>
        /// <param name="x">Indicates whether to home the X-axis. Default is false.</param>
        /// <param name="y">Indicates whether to home the Y-axis. Default is false.</param>
        /// <param name="z">Indicates whether to home the Z-axis. Default is false.</param>
        public void HomeAxisCommand(bool x = false, bool y = false, bool z = false)
        {
            // Send the command to turn off the fan to the printer
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildHomeAxesCommand(x, y, z));
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
            serial.Write(command.ToLower());
            sendCommand = null;
        }


        /// <summary>
        /// Turns off the fan by sending the corresponding command to the printer.
        /// </summary>
        public void SetFanOff()
        {
            // Send the fan speed command to the printer
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFanOffCommand());
        }


        /// <summary>
        /// Sets the fan speed by sending a command to the printer.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void SetFanSpeed(int value)
        {   
            // Send the fan speed command to the printer
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFanSpeedCommand(value));
        }


        /// <summary>
        /// Calculates the fan speed as a percentage based on the provided value.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void CalculateFanSpeedIntoPercentage(double value)
        {
            fanSpeedInPercentage = Math.Round(value / 255 * 100);
        }
    }
}