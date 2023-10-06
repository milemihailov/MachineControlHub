using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp.Print
{
    /// <summary>
    /// Represents a 3D printing job.
    /// </summary>
    public class Print
    {
        public const string DATA_SEPARATOR = "; ";
        public const char NEW_LINE = '\n';
        public const char CARRIAGE_RETURN = '\r';

        /// <summary>
        /// Gets the name of the printed file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the settings extracted from the printed file.
        /// </summary>
        public string ExtractedSettingsFromPrintedFile { get; set; }

        /// <summary>
        /// Gets the start time of the print job.
        /// </summary>
        public DateTime StartTimeOfPrint
        {
            get { return DateTime.UtcNow; }
        }



        /// <summary>
        /// Parses and extracts lines containing specific data from a printed file.
        /// </summary>
        /// <param name="filePath">The path to the printed file to be parsed.</param>
        /// <returns>A string containing lines from the file that match the specified data separator.</returns>
        public void ParseExtractedSettingsFromPrintedFile(string filePath)
        {
            // Open the file for reading
            var file = new StreamReader(filePath);

            // Read the file content and split it into lines
            string[] extractedSettings = file.ReadToEnd().Split(new[] { NEW_LINE, CARRIAGE_RETURN }, StringSplitOptions.RemoveEmptyEntries);

            // Filter lines containing the specified data separator
            var extractedLinesWithSemicolon = extractedSettings.Where(line => line.Contains(DATA_SEPARATOR));

            // Join the filtered lines into a single string
            ExtractedSettingsFromPrintedFile = string.Join(Environment.NewLine, extractedLinesWithSemicolon);
        }


        /// <summary>
        /// Selects and parses the specified G-code file from the SD card.
        /// </summary>
        /// <param name="filePath">The path to the G-code file on the SD card.</param>
        public void SelectAndParseSelectedFile(string filePath)
        {
            // Send a command to select the G-code file on the SD card
            CommandMethods.SendSelectSDFile(filePath);
            File = filePath;
        }

    }

}