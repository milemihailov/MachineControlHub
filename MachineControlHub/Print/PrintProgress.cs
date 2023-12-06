namespace MachineControlHub.Print
{
    /// <summary>
    /// Represents the progress of a 3D printing job.
    /// </summary>
    public class PrintProgress
    {
        /// <summary>
        /// Gets or sets the file being printed.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the name of the currently printing file.
        /// </summary>
        public string PrintingFileName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the printing job.
        /// </summary>
        public DateTime StartTime { get; set; }

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
    }

}