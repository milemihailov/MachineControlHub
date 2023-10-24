using ControllingAndManagingApp.SerialConnection;
using System.Text.RegularExpressions;

namespace ControllingAndManagingApp.Motion

{
    /// <summary>
    /// Represents the available move positions.
    /// </summary>
    public enum MovePositions
    {
        /// <summary>
        /// Represents the X-axis move position.
        /// </summary>
        XMovePos,

        /// <summary>
        /// Represents the Y-axis move position.
        /// </summary>
        YMovePos,

        /// <summary>
        /// Represents the Z-axis move position.
        /// </summary>
        ZMovePos,

        /// <summary>
        /// Represents the extruder (E) move position.
        /// </summary>
        EMovePos
    }

    public enum CurrentPositions
    {
        /// <summary>
        /// Represents the X-axis current position.
        /// </summary>
        XCurrentPos,

        /// <summary>
        /// Represents the Y-axis current position.
        /// </summary>
        YCurrentPos,

        /// <summary>
        /// Represents the Z-axis current position.
        /// </summary>
        ZCurrentPos,

        /// <summary>
        /// Represents the E-axis current position.
        /// </summary>
        ECurrentPos,
    }


    /// <summary>
    /// Represents a position in 3D space, including current and optional move positions.
    /// </summary>
    public class Position
    {

        const string X_COORDINATE_PARSE_PATTERN = @"X:([-\d]+\.\d+)";
        const string Y_COORDINATE_PARSE_PATTERN = @"Y:([-\d]+\.\d+)";
        const string Z_COORDINATE_PARSE_PATTERN = @"Z:([-\d]+\.\d+)";
        const string E_COORDINATE_PARSE_PATTERN = @"E:([-\d]+\.\d+)";
        const string XYZ_COORDINATE_PARSE_PATTERN = @"X:([-\d]+\.\d+) Y:([-\d]+\.\d+) Z:([-\d]+\.\d+)";

        /// <summary>
        /// Gets or sets the current X-axis position.
        /// </summary>
        public double XCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current Y-axis position.
        /// </summary>
        public double YCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current Z-axis position.
        /// </summary>
        public double ZCurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the current extruder (E) position.
        /// </summary>
        public double ECurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the optional X-axis move position.
        /// </summary>
        public double? XMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional Y-axis move position.
        /// </summary>
        public double? YMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional Z-axis move position.
        /// </summary>
        public double? ZMovePosition { get; set; }

        /// <summary>
        /// Gets or sets the optional extruder (E) move position.
        /// </summary>
        public double? EMovePosition { get; set; }


        /// <summary>
        /// Generates a move string based on the specified move position.
        /// </summary>
        /// <param name="position">The desired move position (X, Y, Z, or E).</param>
        /// <returns>A formatted move string (e.g., "X10.0" or "Y5.0").</returns>
        public string XYZEMoveString(MovePositions position)
        {
            switch (position)
            {
                case MovePositions.XMovePos:
                    if (XMovePosition != null)
                    {
                        return $"X{XMovePosition}";
                    }
                    return null;
                case MovePositions.YMovePos:
                    if (YMovePosition != null)
                    {
                        return $"Y{YMovePosition}";
                    }
                    return null;
                case MovePositions.ZMovePos:
                    if (ZMovePosition != null)
                    {
                        return $"Z{ZMovePosition}";
                    }
                    return null;
                case MovePositions.EMovePos:
                    if (EMovePosition != null)
                    {
                        return $"E{EMovePosition}";
                    }
                    return null;
            }
            return null;
        }


        /// <summary>
        /// Parses and retrieves the current position of a specified coordinate (X, Y, Z, or E) from the printer's response.
        /// </summary>
        /// <param name="serial">The serial interface used for communication with the printer.</param>
        /// <param name="position">The coordinate position to retrieve (X, Y, Z, or E).</param>
        /// <exception cref="ArgumentException">Thrown when an invalid or unsupported coordinate position is specified.</exception>
        /// <exception cref="Exception">Thrown when matching the coordinate pattern or parsing the coordinate value fails.</exception>
        public void ParseChosenCurrentPosition(SerialInterface serial, CurrentPositions position)
        {

            // Sleep for a brief moment to ensure the input has enough time to be received and processed.
            // This sleep is used to account for potential delays in serial communication.
            Thread.Sleep(10);


            // Define a regular expression pattern based on the position
            string pattern;

            switch (position)
            {
                case CurrentPositions.XCurrentPos:
                    pattern = X_COORDINATE_PARSE_PATTERN;
                    break;
                case CurrentPositions.YCurrentPos:
                    pattern = Y_COORDINATE_PARSE_PATTERN;
                    break;
                case CurrentPositions.ZCurrentPos:
                    pattern = Z_COORDINATE_PARSE_PATTERN;
                    break;
                case CurrentPositions.ECurrentPos:
                    pattern = E_COORDINATE_PARSE_PATTERN;
                    break;
                default:
                    // Handle the case where position doesn't match any of the coordinate patterns.
                    // You can throw an exception, log an error, or handle it as needed.
                    throw new ArgumentException("Invalid CurrentPositions value.");
            }


            // Read the printer's response

            /// <summary>
            /// Continuously sends a command to request the current position and reads the printer's response until a successful match is found.
            /// </summary>
            /// <param name="serial">The SerialInterface used for communication.</param>
            /// <param name="pattern">The regular expression pattern to match the desired data.</param>
            /// <param name="output">The matched output will be stored in this string.</param>
            string input;
            string output = "";
            while (serial.Read() != null)
            {
                serial.Write(CommandMethods.BuildGetCurrentPositionCommand());

                input = serial.Read();

                // Create a regular expression object

                Match match = Regex.Match(input, pattern);

                if (match.Success)
                {
                    output = match.Groups[1].Value;
                    break;
                }
            }


            if (!double.TryParse(output, out double currentPosition))
            {
                // Handle the case where parsing the output as a double fails.
                // You can throw an exception, log an error, or handle it as needed.
                throw new Exception("Failed to parse the coordinate value.");
            }

            // Assign the parsed coordinate value based on the position
            switch (position)
            {
                case CurrentPositions.XCurrentPos:
                    XCurrentPosition = currentPosition;
                    break;
                case CurrentPositions.YCurrentPos:
                    YCurrentPosition = currentPosition;
                    break;
                case CurrentPositions.ZCurrentPos:
                    ZCurrentPosition = currentPosition;
                    break;
                case CurrentPositions.ECurrentPos:
                    ECurrentPosition = currentPosition;
                    break;
                default:
                    // Handle the case where position doesn't match any of the coordinate patterns.
                    // You can throw an exception, log an error, or handle it as needed.
                    throw new ArgumentException("Invalid CurrentPositions value.");
            }
        }


        /// <summary>
        /// Parses and updates the current X, Y, and Z positions from the printer's response.
        /// </summary>
        /// <param name="serial">The SerialInterface used for communication.</param>
        public void ParseXYZCurrentPositions(SerialInterface serial)
        {
            // Sleep for a brief moment to ensure the input has enough time to be received and processed.
            Thread.Sleep(10);

            // Read the printer's response
            string input;

            while (serial.Read() != null)
            {
                serial.Write(CommandMethods.BuildGetCurrentPositionCommand());
                input = serial.Read();

                // Define a regular expression pattern to match the X, Y, and Z coordinates
                string pattern = XYZ_COORDINATE_PARSE_PATTERN;

                Match match = Regex.Match(input, pattern);

                if (match.Success)
                {
                    // Update the X, Y, and Z current positions
                    XCurrentPosition = double.Parse(match.Groups[1].Value);
                    YCurrentPosition = double.Parse(match.Groups[2].Value);
                    ZCurrentPosition = double.Parse(match.Groups[3].Value);
                    break;
                }
            }
        }
    }

}