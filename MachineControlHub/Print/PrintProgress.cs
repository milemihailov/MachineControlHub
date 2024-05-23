using System.Text.RegularExpressions;


namespace MachineControlHub.Print
{
    /// <summary>
    /// Represents the progress of a 3D printing job.
    /// </summary>
    public class PrintProgress
    {
        const string PATTERN = @"(\S+\.gco) (\d+)";
        const int KILOBYTE = 1024;

        /// <summary>
        /// Gets or sets the name of the currently printing file.
        /// </summary>
        public string PrintingFileName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the printing job.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the total print time for the job in seconds.
        /// </summary>
        public int TotalPrintTime { get; set; }

        /// <summary>
        /// Gets or sets the estimated time remaining for the printing job in seconds.
        /// </summary>
        public int PrintTimeLeft { get; set; }

        /// <summary>
        /// Gets or sets the real-time printing speed in millimeters per second (mm/s).
        /// </summary>
        public double RealTimePrintingSpeed { get; set; }

        /// <summary>
        /// Gets or sets the file size in megabytes.
        /// </summary>
        public double FileSizeInMB {  get; set; }

        public long TotalBytes { get; set; }

        /// <summary>
        /// Parses the file name and calculates the size in megabytes.
        /// </summary>
        /// <param name="input">The input string containing the file name and size.</param>
        public void ParseFileNameAndSize(string input)
        {
            Match match = Regex.Match(input, PATTERN, RegexOptions.IgnoreCase);
            PrintingFileName = match.Groups[1].Value;
            FileSizeInMB = ConvertToMB(double.Parse(match.Groups[2].Value));
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


        public List<PrintProgressRecord> PrintProgressRecords { get; set; } = new List<PrintProgressRecord>();
        public TimeSpan EstimateTimeRemaining()
        {
            if (PrintProgressRecords.Count < 5)
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
                totalBytesPrinted += currentRecord.BytesPrinted - previousRecord.BytesPrinted;
                totalTimeElapsed += currentRecord.Timestamp - previousRecord.Timestamp;
            }

            var averageSpeed = totalBytesPrinted / totalTimeElapsed.TotalSeconds; // bytes per second
            var remainingBytes = TotalBytes - PrintProgressRecords.Last().BytesPrinted;
            var remainingTimeInSeconds = remainingBytes / averageSpeed;
            return TimeSpan.FromSeconds(Math.Round(remainingTimeInSeconds));
        }

    }

}