using ControllingAndManagingApp.Motion;
using System.Text.RegularExpressions;

namespace ControllingAndManagingApp.Temps
{
    /// <summary>
    /// Represents the Proportional-Integral-Derivative (PID) values used for temperature control.
    /// </summary>
    public class PIDValues
    {
        const string PARSE_PID_PATTERN = @"M301 P(\d+\.\d+) I(\d+\.\d+) D(\d+\.\d+)";

        /// <summary>
        /// Gets or sets the Proportional (P) value for PID control.
        /// </summary>
        public double Proportional { get; set; }

        /// <summary>
        /// Gets or sets the Integral (I) value for PID control.
        /// </summary>
        public double Integral { get; set; }

        /// <summary>
        /// Gets or sets the Derivative (D) value for PID control.
        /// </summary>
        public double Derivative { get; set; }


        /// <summary>
        /// Parses PID values from the serial input after sending a command to report settings.
        /// </summary>
        /// <param name="serial">The SerialInterface object used for communication.</param>
        /// <remarks>
        /// This method sends a command to the serial interface to report settings and then reads the
        /// response to extract and parse the Proportional (P), Integral (I), and Derivative (D) values
        /// related to the PID controller.
        /// </remarks>
        /// <seealso cref="CommandMethods.BuildReportSettings"/>
        /// <seealso cref="Proportional"/>
        /// <seealso cref="Integral"/>
        /// <seealso cref="Derivative"/>
        public void ParsePIDValues(PrinterConnection.SerialConnection serial)
        {
            // Send a command to report settings
            serial.Write(CommandMethods.BuildReportSettings());

            // Wait for the response
            Thread.Sleep(200);

            // Read the response from the serial interface
            string input = serial.Read();

            // Match the response with the PID pattern
            Match match = Regex.Match(input, PARSE_PID_PATTERN);

            // If a match is found, extract and parse the PID values
            if (match.Success)
            {
                string pValue = match.Groups[1].Value;
                string iValue = match.Groups[2].Value;
                string dValue = match.Groups[3].Value;

                // Parse and assign the Proportional, Integral, and Derivative values
                Proportional = double.Parse(pValue);
                Integral = double.Parse(iValue);
                Derivative = double.Parse(dValue);
            }
            else
            {
                Console.WriteLine("Value not found in the string");
            }
        }
    }

}