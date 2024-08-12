using MachineControlHub;
using MachineControlHub.Motion;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plotly.Blazor.Interop;
using System.Text.RegularExpressions;

namespace WebUI.Data
{
    public class ControlPanelService
    {
        private const string _fAN_PATTERN = @"M106 P0 S(\d+)";

        /// <summary>
        /// Stores the console output messages.
        /// </summary>
        public string ConsoleOutput { get; set; }

        /// <summary>
        /// Stores the fan speed in percentage.
        /// </summary>
        public double FanSpeedInPercentage { get; set; }

        /// <summary>
        /// Command to send to the printer via the terminal.
        /// </summary>
        public string SendCommand { get; set; } = "";

        /// <summary>
        /// Value to move the axis by when adjusting the position.
        /// </summary>
        public double ValueToMove { get; set; } = 10;

        /// <summary>
        /// Stores the value of the fan speed slider.
        /// </summary>
        public int FanSpeedValue { get; set; }

        /// <summary>
        /// Stores the value of the fan toggle button.
        /// </summary>
        public bool ToggleFanValue { get; set; } = false;

        /// <summary>
        /// Indicates whether to show SD card related messages in the console window.
        /// </summary>
        public bool ShowSDMessages { get; set; }

        /// <summary>
        /// Indicates whether to show temperature related messages in the console window.
        /// </summary>
        public bool ShowTemperatureMessages { get; set; }

        /// <summary>
        /// Indicates whether to show position related messages in the console window.
        /// </summary>
        public bool ShowPositionMessages { get; set; }

        /// <summary>
        /// Indicates whether to show busy or processing messages in the console window.
        /// </summary>
        public bool ShowBusyMessages { get; set; }

        public int Index = -1;

        /// <summary>
        /// Adjusts the specified axis position by the given value.
        /// </summary>
        /// <param name="position">The axis to adjust.</param>
        /// <param name="increment">True to increment, false to decrement.</param>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void AdjustAxis(MovePositions position, bool increment, Printer printer)
        {
            // Reset the move positions
            printer.Position.XMovePosition = null;
            printer.Position.YMovePosition = null;
            printer.Position.ZMovePosition = null;
            printer.Position.EMovePosition = null;

            // Determine the movement value based on whether it's an increment or decrement
            double moveValue = increment ? ValueToMove : -ValueToMove;

            // Set the appropriate property in the Position object based on the specified axis
            switch (position)
            {
                case MovePositions.XMovePos:
                    printer.Position.XMovePosition = moveValue;
                    break;

                case MovePositions.YMovePos:
                    printer.Position.YMovePosition = moveValue;
                    break;

                case MovePositions.ZMovePos:
                    printer.Position.ZMovePosition = moveValue;
                    break;

                case MovePositions.EMovePos:
                    printer.Position.EMovePosition = moveValue;
                    break;
            }
            // Send a command to set relative positioning
            printer.SerialConnection.Write(CommandMethods.BuildRelativePositionCommand());

            // Send a command to perform a linear move with the specified values and feed rate
            printer.SerialConnection.Write(CommandMethods.BuildLinearMoveCommand(printer.Position, printer.MotionSettings));
            printer.SerialConnection.Write(CommandMethods.BuildAbsolutePositionCommand());

            // Request the current positions of the printer
            printer.SerialConnection.Write(CommandMethods.BuildRequestCurrentPositionsCommand());
        }

        /// <summary>
        /// Sends a baby stepping command to the printer.
        /// </summary>
        /// <param name="input">The baby stepping value.</param>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void BabySteps(double input, Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildBabySteppingCommand(input));
        }

        /// <summary>
        /// Sends a command to auto-align the Z steppers of the printer.
        /// </summary>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void ZSteppersAutoAlignment(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildAdjustDualZMotorCommand());
        }

        /// <summary>
        /// Sends a command to disable the steppers of the 3D printer.
        /// </summary>
        public void DisableSteppers(Printer printer)
        {
            // Send the command to disable the steppers to the printer
            printer.SerialConnection.Write(CommandMethods.BuildDisableSteppersCommand());
        }

        /// <summary>
        /// Sends a command to home one or more axes of the 3D printer.
        /// </summary>
        /// <param name="x">Indicates whether to home the X-axis. Default is false.</param>
        /// <param name="y">Indicates whether to home the Y-axis. Default is false.</param>
        /// <param name="z">Indicates whether to home the Z-axis. Default is false.</param>
        public void HomeAxisCommand(Printer printer, bool x = false, bool y = false, bool z = false)
        {
            // Send the command to turn off the fan to the printer
            printer.SerialConnection.Write(CommandMethods.BuildHomeAxesCommand(x, y, z));
        }

        /// <summary>
        /// Sends a Gcode command via the terminal connection.
        /// </summary>
        /// <param name="command">The Gcode command to send.</param>
        /// <param name="serial">The serial connection service.</param>
        /// <remarks>
        /// The provided Gcode command will be sent to the terminal connection after converting it to lowercase.
        /// </remarks>
        public void SendGcodeViaTerminal(string command, Printer printer)
        {
            printer.SerialConnection.Write(command);
            SendCommand = null;
        }

        /// <summary>
        /// Sets the fan speed by sending a command to the printer.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void SetFanSpeed(int value, Printer printer)
        {
            // Send the fan speed command to the printer
            printer.SerialConnection.Write(CommandMethods.BuildFanSpeedCommand(value));
            if (value > 0)
            {
                ToggleFanValue = true;
            }
        }

        /// <summary>
        /// Sets the print speed percentage by sending a command to the printer.
        /// </summary>
        /// <param name="feedrate">The feedrate value to set (percentage).</param>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void SetPrintSpeedPercentage(int feedrate, Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildSetPrintSpeedCommand(feedrate));
            printer.MotionSettings.PrintSpeed = feedrate;
        }

        /// <summary>
        /// Sets the print flow percentage by sending a command to the printer.
        /// </summary>
        /// <param name="flowrate">The flowrate value to set (percentage).</param>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void SetPrintFlowPercentage(int flowrate, Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildSetPrintFlowCommand(flowrate));
            printer.MotionSettings.PrintFlow = flowrate;
        }

        /// <summary>
        /// Calculates the fan speed as a percentage based on the provided value.
        /// </summary>
        /// <param name="value">The fan speed value (0 to 255).</param>
        public void CalculateFanSpeedIntoPercentage(double value)
        {
            FanSpeedInPercentage = Math.Round(value / 255 * 100);
        }
        /// <summary>
        /// Updates the console output with the provided message after processing it.
        /// </summary>
        /// <param name="message">The message to process and update.</param>
        /// <param name="printer">The printer object containing the serial connection.</param>
        public void UpdateParagraph(string input, Printer printer)
        {
            double value;

            // Check if the message matches the fan speed pattern
            Match match = Regex.Match(input, _fAN_PATTERN);
            if (match.Success)
            {
                // Extract the fan speed value and calculate its percentage
                value = double.Parse(match.Groups[1].Value);
                CalculateFanSpeedIntoPercentage(value);
                input = $"Fan Speed: {FanSpeedInPercentage}%\n";
            }

            // Filter out SD card related messages if ShowSDMessages is false
            if (!ShowSDMessages && (input.Contains("Not SD printing") || input.Contains("printing byte"))) 
            {
                input = null;
            }

            // Filter out temperature related messages if ShowTemperatureMessages is false
            if (!ShowTemperatureMessages && Regex.IsMatch(input, @"T:-?\d+\.\d+ /-?0\.00"))
            {
                input = null;
            }

            // Filter out position related messages if ShowPositionMessages is false
            if (!ShowPositionMessages && (input.Contains("Count")))
            {
                input = null;
            }

            // Filter out busy or processing messages if ShowBusyMessages is false
            if (!ShowBusyMessages && (input.Contains("busy") || input.Contains("processing")))
            {
                input = null;
            }

            // If the message is not null and is not just "ok", process it further
            if (input != null && input.Trim() != "ok")
            {
                ConsoleOutput += $"{printer.SerialConnection.PortName}: {input}";

                int newlineCount = ConsoleOutput.Count(c => c == '\n');

                // Avoids infinite appending to ConsoleOutput
                if(newlineCount > 500)
                {
                    // Split the ConsoleOutput into lines
                    var lines = ConsoleOutput.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // Remove the first 100 lines
                    lines = lines.Skip(100).ToList();

                    // Join the remaining lines back into the ConsoleOutput string
                    ConsoleOutput = string.Join("\n", lines) + "\n";
                }
            }
        }

        /// <summary>
        /// Toggles the visibility of SD card related messages.
        /// </summary>
        public void ToggleSDMessages()
        {
            ShowSDMessages = !ShowSDMessages;
        }

        /// <summary>
        /// Toggles the visibility of temperature related messages.
        /// </summary>
        public void ToggleTemperatureMessages()
        {
            ShowTemperatureMessages = !ShowTemperatureMessages;
        }

        /// <summary>
        /// Toggles the visibility of position related messages.
        /// </summary>
        public void TogglePositionMessages()
        {
            ShowPositionMessages = !ShowPositionMessages;
        }

        /// <summary>
        /// Toggles the visibility of busy or processing messages.
        /// </summary>
        public void ToggleBusyMessages()
        {
            ShowBusyMessages = !ShowBusyMessages;
        }

        /// <summary>
        /// Toggles the fan on or off and sets the fan speed accordingly.
        /// </summary>
        public void ToggleFan()
        {
            ToggleFanValue = !ToggleFanValue;
            if (ToggleFanValue)
            {
                FanSpeedValue = 255;

            }
            else
            {
                FanSpeedValue = 0;
            }
        }
    }
}