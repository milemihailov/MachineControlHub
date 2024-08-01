using MachineControlHub;
using MachineControlHub.Motion;
using Microsoft.JSInterop;
using Plotly.Blazor.Interop;
using System.Text.RegularExpressions;

namespace WebUI.Data
{
    public class ControlPanelService
    {
        public const string FAN_PATTERN = @"M106 P0 S(\d+)";

        public string ConsoleOutput { get; set; }
        public int DefaultFanSpeed { get; set; }
        public double FanSpeedInPercentage { get; set; }
        public string SendCommand { get; set; } = "";
        public double ValueToMove { get; set; } = 10;
        public bool SwitchValue { get; set; } = false;
        public int FanSpeed { get; set; }
        public int? FreeMoveFeedRate { get; set; }
        public int FeedRatePercentage { get; set; } = 100;
        public int PrintFlowPercentage { get; set; } = 100;

        public MotionSettingsData FeedRate { get; set; }
        public Position PositionToMove { get; set;}
        public PrinterManagerService Printer { get; set; }
        public ControlPanelService(PrinterManagerService Printer)
        {
            this.Printer = Printer;
            FeedRate = new MotionSettingsData();
            PositionToMove = new Position();
        }

        /// <summary>
        /// Adjusts the specified axis position by the given value.
        /// </summary>
        /// <param name="position">The axis to adjust.</param>
        /// <param name="increment">True to increment, false to decrement.</param>
        public void AdjustAxis(MovePositions position, bool increment)
        {
            // Create a new Position object to store the movement values
            Position pos = new();

            // Send a command to set relative positioning
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildRelativePositionCommand());

            // Determine the movement value based on whether it's an increment or decrement
            double moveValue = increment ? ValueToMove : -ValueToMove;

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
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildLinearMoveCommand(pos, FeedRate));
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildAbsolutePositionCommand());
        }

        public void BabySteps(double input)
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildBabySteppingCommand(input));
        }

        public void ZSteppersAutoAlignment()
        {

            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildAdjustDualZMotorCommand());
        }


        public void AdjustFreeMoveFeedRate()
        {
            FreeMoveFeedRate = FeedRate.FeedRateFreeMove;
        }

        /// <summary>
        /// Sends a command to disable the steppers of the 3D printer.
        /// </summary>
        public void DisableSteppers()
        {
            // Send the command to disable the steppers to the printer
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildDisableSteppersCommand());
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
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildHomeAxesCommand(x, y, z));
        }


        /// <summary>
        /// Sends a Gcode command via the terminal connection.
        /// </summary>
        /// <param name="command">The Gcode command to send.</param>
        /// <param name="serial">The serial connection service.</param>
        /// <remarks>
        /// The provided Gcode command will be sent to the terminal connection after converting it to lowercase.
        /// </remarks>
        public void SendGcodeViaTerminal(string command)
        {
            Printer.ActivePrinter.SerialConnection.Write(command);
            SendCommand = null;
        }


        /// <summary>
        /// Sets the fan speed by sending a command to the printer.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void SetFanSpeed(int value)
        {
            // Send the fan speed command to the printer
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildFanSpeedCommand(value));
            if (value > 0)
            {
                SwitchValue = true;
            }
        }

        public void SetFeedRatePercentage(int feedrate)
        {
            Printer.ActivePrinter.SerialConnection.Write($"M220 S{feedrate}");
        }

        public void SetPrintFlowPercentage(int flowrate)
        {
            Printer.ActivePrinter.SerialConnection.Write($"M221 S{flowrate}");
        }

        /// <summary>
        /// Calculates the fan speed as a percentage based on the provided value.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void CalculateFanSpeedIntoPercentage(double value)
        {
            FanSpeedInPercentage = Math.Round(value / 255 * 100);
        }

        public void UpdateParagraph(string message)
        {
            double value;
            Match match = Regex.Match(message, FAN_PATTERN);
            if (match.Success)
            {
                value = double.Parse(match.Groups[1].Value);
                CalculateFanSpeedIntoPercentage(value);
                message = $"Fan Speed: {FanSpeedInPercentage}%\n";
            }
            if (message.Contains("Not SD printing") || message.Contains("printing byte") || Regex.IsMatch(message, @"T:\d+\.\d+ /0\.00"))
            {
                message = null;
            }

            if (message != null)
            {

                var lines = message.Split('\n');
                var filteredLines = lines.Where(line => !line.Contains("ok"));
                var filteredData = string.Join('\n', filteredLines);
                ConsoleOutput += filteredData;

            }

            //try
            //{
            //    var isAtBottom = await JSRuntime.InvokeAsync<bool>("isAtBottom", "elementId");
            //    if (isAtBottom)
            //    {
            //        await JSRuntime.InvokeVoidAsync("scrollToBottom");
            //    }
            //}
            //catch (TaskCanceledException)
            //{

            //}
        }

        public void ToggleValue()
        {
            SwitchValue = !SwitchValue;
            if (SwitchValue)
            {
                FanSpeed = 255;
                DefaultFanSpeed = 255;

            }
            else
            {
                FanSpeed = 0;
                DefaultFanSpeed = 0;
            }

        }
    }
}