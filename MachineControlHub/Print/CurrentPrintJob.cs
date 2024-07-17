using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MachineControlHub.Print
{

    /// <summary>
    /// Represents a 3D printing job.
    /// </summary>
    public class CurrentPrintJob
    {
        public List<PrintProgressRecord> PrintProgressRecords;

        const string PATTERN = @"(\S+\.gco) (\d+)";
        const int KILOBYTE = 1024;

        public const string DATA_SEPARATOR = "; ";
        public const char NEW_LINE = '\n';
        public const char CARRIAGE_RETURN = '\r';

        private IPrinterConnection _connection;

        public CurrentPrintJob(IPrinterConnection connection)
        {
            _connection = connection;
            PrintProgressRecords = new List<PrintProgressRecord>();
        }
        public CurrentPrintJob()
        {

        }
        public string PrinterName { get; set; }

        /// <summary>
        /// Gets the name of the printed file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the settings extracted from the printed file.
        /// </summary>
        public string ExtractedSettingsFromPrintedFile { get; set; }

        /// <summary>
        /// Gets the start time of the print job.
        /// </summary>

        public DateTime? StartTimeOfPrint { get; set; }


        /// <summary>
        /// Gets or sets the name of the currently printing file.
        /// </summary>
        public string PrintingFileName { get; set; }

        /// <summary>
        /// Gets or sets the total print time for the job in seconds.
        /// </summary>
        public string TotalPrintTime { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Gets or sets the real-time printing speed in millimeters per second (mm/s).
        /// </summary>
        public double RealTimePrintingSpeed { get; set; }

        /// <summary>
        /// Gets or sets the file size in megabytes.
        /// </summary>
        public double FileSize { get; set; }

        public long TotalBytes { get; set; }

        public long CurrentBytes { get; set; }

        /// <summary>
        /// Parses and extracts lines containing specific data from a printed file.
        /// </summary>
        /// <param name="text">The path to the printed file to be parsed.</param>
        /// <returns>A string containing lines from the file that match the specified data separator.</returns>
        public void ParseExtractedSettingsFromPrintedFile(string text)
        {
            try
            {
                string[] extractedSettings = text.Split(new[] { NEW_LINE, CARRIAGE_RETURN }, StringSplitOptions.RemoveEmptyEntries);

                var extractedLinesWithSemicolon = extractedSettings.Where(line => line.Contains(DATA_SEPARATOR));

                ExtractedSettingsFromPrintedFile = string.Join(Environment.NewLine, extractedLinesWithSemicolon);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or return an error message
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        /// <summary>
        /// Selects and parses the specified G-code file from the SD card.
        /// </summary>
        /// <param name="filePath">The path to the G-code file on the SD card.</param>
        public void SelectAndParseSelectedFile(string filePath)
        {
            // Send a command to select the G-code file on the SD card
            _connection.Write(CommandMethods.BuildSelectSDFileCommand(filePath));
            FileName = filePath;
        }


        /// <summary>
        /// Parses and sets the start time of the print job to the current system time.
        /// </summary>
        public void ParseStartTimeOfPrint()
        {
            StartTimeOfPrint = DateTime.Now;
        }

        public string FormattedStartTime
        {
            get
            {
                return StartTimeOfPrint.HasValue ? StartTimeOfPrint.Value.ToString("dd MMMM yyyy HH:mm:ss") : "-/-";
            }
        }


        /// <summary>
        /// Converts the size from megabytes to kilobytes.
        /// </summary>
        /// <param name="megaByte">The size in megabytes.</param>
        /// <returns>The size in kilobytes.</returns>
        public double ConvertToMB(double megaByte)
        {
            return megaByte / (KILOBYTE * KILOBYTE);
        }

        public class PrintProgressRecord
        {
            public long BytesPrinted { get; set; }
            public DateTime Timestamp { get; set; }
        }


        public async Task<TimeSpan> EstimateTimeRemainingAsync()
        {
            return await Task.Run(() =>
            {
                if (PrintProgressRecords == null || PrintProgressRecords.Count < 5)
                {
                    // Not enough data to estimate time remaining
                    return TimeSpan.Zero;
                }

                var totalBytesPrinted = 0L;
                var totalTimeElapsed = TimeSpan.Zero;

                for (int i = 1; i < PrintProgressRecords.Count; i++)
                {
                    var currentRecord = PrintProgressRecords[i];
                    var previousRecord = PrintProgressRecords[i - 1];
                    if (currentRecord != null && previousRecord != null)
                    {
                        totalBytesPrinted += currentRecord.BytesPrinted - previousRecord.BytesPrinted;
                        totalTimeElapsed += currentRecord.Timestamp - previousRecord.Timestamp;
                    }
                }

                var averageSpeed = totalBytesPrinted / totalTimeElapsed.TotalSeconds; // bytes per second
                var remainingBytes = TotalBytes - (PrintProgressRecords.LastOrDefault()?.BytesPrinted ?? 0);
                var remainingTimeInSeconds = remainingBytes / averageSpeed;
                return TimeSpan.FromSeconds(Math.Round(remainingTimeInSeconds));
            });
        }

    }

}