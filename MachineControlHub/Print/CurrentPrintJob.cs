using System.Diagnostics;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Print
{

    /// <summary>
    /// Represents a 3D printing job.
    /// </summary>
    public class CurrentPrintJob
    {
        public List<PrintProgressRecord> PrintProgressRecords;

        const int _kILOBYTE = 1024;
        const string _dATA_SEPARATOR = "; ";
        const char _nEW_LINE = '\n';
        const char _cARRIAGE_RETURN = '\r';

        public Stopwatch Stopwatch { get; set; } = new();
        private IPrinterConnection Connection { get; set; }

        public CurrentPrintJob(IPrinterConnection connection)
        {
            Connection = connection;
            PrintProgressRecords = new List<PrintProgressRecord>();
        }

        /// <summary>
        /// Parameterless constructor to facilitate deserialization
        /// </summary>
        public CurrentPrintJob()
        {
            PrintProgressRecords = new List<PrintProgressRecord>();
        }


        public string PortName { get; set; }

        public string PrinterName { get; set; }

        /// <summary>
        /// Gets the name of the printed file.
        /// </summary>
        public string FileName { get; set; }

        public double PrintProgress { get; set; } = 0;


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

        public TimeSpan EstimatePrintTime { get; set; }

        /// <summary>
        /// Gets or sets the total print time for the job in seconds.
        /// </summary>
        public string TotalPrintTime { get; set; }

        /// <summary>
        /// Gets or sets the real-time printing speed in millimeters per second (mm/s).
        /// </summary>
        public double RealTimePrintingSpeed { get; set; }

        public bool IsPrinting { get; set; }

        /// <summary>
        /// Gets or sets the file size in megabytes.
        /// </summary>
        public double FileSize { get; set; }

        public long TotalBytes { get; set; }

        public long CurrentBytes { get; set; }

        /// <summary>
        /// A flag indicating whether the finalization steps of the print job have been executed.
        /// </summary>
        public bool FinalizationExecuted { get; set; } = false;


        /// <summary>
        /// Parses and extracts lines containing specific data from a printed file.
        /// </summary>
        /// <param name="text">The path to the printed file to be parsed.</param>
        /// <returns>A string containing lines from the file that match the specified data separator.</returns>
        public void ParseExtractedSettingsFromPrintedFile(string text)
        {
            try
            {
                string[] extractedSettings = text.Split(new[] { _nEW_LINE, _cARRIAGE_RETURN }, StringSplitOptions.RemoveEmptyEntries);

                var extractedLinesWithSemicolon = extractedSettings.Where(line => line.Contains(_dATA_SEPARATOR));

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
        /// May be used in future...
        /// </summary>
        /// <param name="filePath">The path to the G-code file on the SD card.</param>
        public void SelectAndParseSelectedFile(string filePath)
        {
            // Send a command to select the G-code file on the SD card
            Connection.Write(CommandMethods.BuildSelectSDFileCommand(filePath));
            FileName = filePath;
        }


        /// <summary>
        /// Parses and sets the start time of the print job to the current system time.
        /// </summary>
        public void ParseStartTimeOfPrint()
        {
            StartTimeOfPrint = DateTime.Now;
        }

        /// <summary>
        /// Gets the formatted start time of the print job.
        /// </summary>
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
        public double ConvertToMB(double megaByte) => megaByte / (_kILOBYTE * _kILOBYTE);

        /// <summary>
        /// Estimates the remaining time for the print job based on the progress records.
        /// </summary>
        public async Task EstimateTimeRemainingAsync()
        {
            // Run the estimation logic in a separate task to avoid blocking the main thread.
            await Task.Run(() =>
            {
                // Check if there are enough progress records to make a reliable estimate.
                if (PrintProgressRecords == null || PrintProgressRecords.Count < 5)
                {
                    // Not enough data to estimate time remaining
                    EstimatePrintTime = TimeSpan.Zero;
                }

                // Initialize variables to accumulate total bytes printed and total time elapsed.
                var totalBytesPrinted = 0L;
                var totalTimeElapsed = TimeSpan.Zero;

                // Iterate through the progress records starting from the second record.
                for (int i = 1; i < PrintProgressRecords.Count; i++)
                {
                    var currentRecord = PrintProgressRecords[i];
                    var previousRecord = PrintProgressRecords[i - 1];
                    if (currentRecord != null && previousRecord != null)
                    {
                        // Accumulate the bytes printed and time elapsed between consecutive records.
                        totalBytesPrinted += currentRecord.BytesPrinted - previousRecord.BytesPrinted;
                        totalTimeElapsed += currentRecord.Timestamp - previousRecord.Timestamp;
                    }
                }
                if (totalTimeElapsed.TotalSeconds > 0)
                {
                    var averageSpeed = totalBytesPrinted / totalTimeElapsed.TotalSeconds;
                    var remainingBytes = TotalBytes - (PrintProgressRecords.LastOrDefault()?.BytesPrinted ?? 0);
                    var remainingTimeInSeconds = remainingBytes / averageSpeed;
                    EstimatePrintTime = TimeSpan.FromSeconds(Math.Round(remainingTimeInSeconds));
                }
                else
                {
                    EstimatePrintTime = TimeSpan.Zero; // or another default value
                }
            });
        }

        public class PrintProgressRecord
        {
            public long BytesPrinted { get; set; }
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// Starts the stopwatch to measure the print time.
        /// </summary>
        public void StartStopwatch()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        /// <summary>
        /// Resets the stopwatch.
        /// </summary>
        public void ResetStopwatch()
        {
            Stopwatch.Reset();
        }

        /// <summary>
        /// Stops the stopwatch.
        /// </summary>
        public void StopStopwatch()
        {
            Stopwatch.Stop();
        }
    }
}