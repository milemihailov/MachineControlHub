using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Temps

{

    /// <summary>
    /// Represents temperature-related information for the printer's hotend.
    /// </summary>
    public class HotendTemps
    {
        const string HOTEND_TEMP_PARSE_PATTERN = @"T:(\d+)\.\d+\s*/(\d+)\.\d+";
        private IPrinterConnection _connection;

        public HotendTemps(IPrinterConnection connection)
        {
            _connection = connection;
        }


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
        public int SetHotendTemp { get; set; }

        public int TargetHotendTemp { get; set; }


        /// <summary>
        /// Parses the current hotend temperature from a provided input string.
        /// </summary>
        /// <remarks>
        /// This method sends a command to report temperatures to the provided serial interface,
        /// then extracts and parses the hotend temperature value from the received input string.
        /// The extracted temperature value is stored in the HotendCurrentTemp property.
        /// </remarks>
        public void ParseCurrentHotendTemperature()
        {
            Thread.Sleep(200);  // Simulating the initial delay asynchronously

            // Send a command to request temperature information
            _connection.Write(CommandMethods.BuildReportTemperaturesCommand());

            // Simulate the delay asynchronously
            Thread.Sleep(200);

            // Read the printer's response
            string input = _connection.Read();

            // Define a regular expression pattern to match the bed temperature
            string pattern = HOTEND_TEMP_PARSE_PATTERN;

            // Create a regular expression object and find matches in the input string
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            // Iterate through the matches and extract the temperature values
            foreach (Match match in matches)
            {
                // The temperature value is captured in the first group (index 1)
                HotendCurrentTemp = int.Parse(match.Groups[1].Value);
                TargetHotendTemp = int.Parse(match.Groups[2].Value);
            }
        }


        /// <summary>
        /// Sets the target temperature for the hotend and sends the corresponding G-code command to the printer.
        /// </summary>
        /// <param name="temp">The target temperature to set for the hotend.</param>
        public void SetHotendTemperature(int setTemp)
        {
            // Send the G-code command to set the hotend temperature
            _connection.Write(CommandMethods.BuildSetHotendTempCommand(setTemp));
        }
    }

}
