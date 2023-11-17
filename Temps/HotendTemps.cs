using ControllingAndManagingApp.Motion;
using System.Text.RegularExpressions;

namespace ControllingAndManagingApp.Temps

{

    /// <summary>
    /// Represents temperature-related information for the printer's hotend.
    /// </summary>
    public class HotendTemps
    {
        const string HOTEND_TEMP_PARSE_PATTERN = @"T:(\d+)";

        /// <summary>
        /// Gets or sets the PID (Proportional-Integral-Derivative) values for controlling the hotend temperature.
        /// </summary>
        public PIDValues PIDHotendValues { get; set; }

        /// <summary>
        /// Gets or sets the current temperature of the hotend in degrees Celsius.
        /// </summary>
        public int HotendCurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the hotend in degrees Celsius.
        /// </summary>
        public int HotendMaxTemp { get; set; }

        /// <summary>
        /// Gets or sets the target temperature set for the hotend in degrees Celsius.
        /// </summary>
        public int HotendSetTemp { get; set; }


        /// <summary>
        /// Parses the current hotend temperature from a provided input string.
        /// </summary>
        /// <param name="serial">The serial interface for communication.</param>
        /// <remarks>
        /// This method sends a command to report temperatures to the provided serial interface,
        /// then extracts and parses the hotend temperature value from the received input string.
        /// The extracted temperature value is stored in the HotendCurrentTemp property.
        /// </remarks>
        public void ParseCurrentHotendTemp(PrinterConnection.SerialConnection serial)
        {
            // Send a command to request temperature information
            serial.Write(CommandMethods.BuildReportTemperaturesCommand());

            // Sleep for a brief moment to ensure the input has enough time to be received and processed.
            // This sleep is used to account for potential delays in serial communication.
            Thread.Sleep(200);

            // Read the printer's response
            string input = serial.Read();

            // Define a regular expression pattern to match the bed temperature
            string pattern = HOTEND_TEMP_PARSE_PATTERN;

            // Create a regular expression object and find matches in the input string
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            // Iterate through the matches and extract the temperature values
            foreach (Match match in matches)
            {
                // The temperature value is captured in the first group (index 1)
                string temperature = match.Groups[1].Value;
                HotendCurrentTemp = int.Parse(temperature);
            }
        }


        /// <summary>
        /// Sets the target temperature for the hotend and sends the corresponding G-code command to the printer.
        /// </summary>
        /// <param name="serial">The serial interface used for communication with the printer.</param>
        /// <param name="temp">The target temperature to set for the hotend.</param>
        public void SetHotendTemp(PrinterConnection.SerialConnection serial, int targetTemp)
        {
            // Send the G-code command to set the hotend temperature
            serial.Write(CommandMethods.BuildSetHotendTempCommand(targetTemp));

            // Update the HotendSetTemp property with the new target temperature
            HotendSetTemp = targetTemp;
        }
    }

}
