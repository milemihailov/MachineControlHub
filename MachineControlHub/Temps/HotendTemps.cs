using MachineControlHub.Motion;
using System.Text.RegularExpressions;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Temps

{

    /// <summary>
    /// Represents temperature-related information for the printer's hotend.
    /// </summary>
    public class HotendTemps : ITemperatures
    {
        const string _hOTEND_TEMP_PARSE_PATTERN = @"T:(\d+)\.\d+\s*/(\d+)\.\d+";
        private IPrinterConnection Connection { get; set; }

        public HotendTemps(IPrinterConnection connection)
        {
            Connection = connection;
            PIDValues = new PIDValues(connection);
        }


        /// <summary>
        /// Gets or sets the PID (Proportional-Integral-Derivative) values for controlling the hotend temperature.
        /// </summary>
        public PIDValues PIDValues { get; set; }


        public int CurrentTemp { get ; set ; }
        public int MaxTemp { get; set; }
        public int SetTemp { get; set; }
        public int TargetTemp { get; set; }


        /// <summary>
        /// Parses the current hotend temperature from a provided input string.
        /// </summary>
        /// <remarks>
        /// This method sends a command to report temperatures to the provided serial interface,
        /// then extracts and parses the hotend temperature value from the received input string.
        /// The extracted temperature value is stored in the HotendCurrentTemp property.
        /// </remarks>
        public void ParseCurrentTemperature(string input)
        {
            // Define a regular expression pattern to match the bed temperature
            string pattern = _hOTEND_TEMP_PARSE_PATTERN;

            // Create a regular expression object and find matches in the input string

            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                CurrentTemp = int.Parse(match.Groups[1].Value);
                TargetTemp = int.Parse(match.Groups[2].Value);
            }
        }


        /// <summary>
        /// Sets the target temperature for the hotend and sends the corresponding G-code command to the printer.
        /// </summary>
        /// <param name="temp">The target temperature to set for the hotend.</param>
        public void SetTemperature(int setTemp)
        {
            // Send the G-code command to set the hotend temperature
            Connection.Write(CommandMethods.BuildSetHotendTempCommand(setTemp));
        }
    }

}
