using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using System.Text.RegularExpressions;

namespace MachineControlHub.Temps
{

    /// <summary>
    /// Represents temperature-related information for the printer's heated bed.
    /// </summary>
    public class BedTemps : ITemperatures
    {
        const string BED_TEMP_PARSE_PATTERN = @"B:(\d+)\.\d+\s*/(\d+)\.\d+";

        private IPrinterConnection _connection;

        public BedTemps(IPrinterConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Gets or sets the PID (Proportional-Integral-Derivative) control values for the bed temperature.
        /// </summary>
        public PIDValues PIDBedValues { get; set; }

        public int CurrentTemp { get; set; }
        public int MaxTemp { get; set; }
        public int SetTemp { get; set; }
        public int TargetTemp { get; set; }


        /// <summary>
        /// Parses and updates the current bed temperature from the response received from the printer.
        /// </summary>
        /// <param name="serial">The serial interface used for communication with the printer.</param>
        public void ParseCurrentTemperature(string input)
        {
            //Thread.Sleep(200);
            //// Send a command to request temperature information
            //_connection.Write(CommandMethods.BuildReportTemperaturesCommand());

            //// Sleep for a brief moment to ensure the input has enough time to be received and processed.
            //// This sleep is used to account for potential delays in serial communication.
            //Thread.Sleep(200);

            //// Read the printer's response
            //string input = _connection.Read();

            // Define a regular expression pattern to match the bed temperature
            string pattern = BED_TEMP_PARSE_PATTERN;

            // Create a regular expression object and find matches in the input string
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            // Iterate through the matches and extract the temperature values
            foreach (Match match in matches)
            {
                // The temperature value is captured in the first group (index 1)
                CurrentTemp = int.Parse(match.Groups[1].Value);
                TargetTemp = int.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// Sets the target bed temperature and sends the corresponding command to the printer.
        /// </summary>
        /// <param name="serial">The serial interface used for communication with the printer.</param>
        /// <param name="targetTemp">The target bed temperature to set.</param>
        public void SetTemperature(int setTemp)
        {
            // Send a command to set the target bed temperature
            _connection.Write(CommandMethods.BuildSetBedTempCommand(setTemp));
        }

    }

}