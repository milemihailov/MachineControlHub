namespace MachineControlHub.Print
{
    /// <summary>
    /// Represents a history of 3D printing jobs.
    /// </summary>
    public class PrintHistory
    {
        /// <summary>
        /// Gets or sets the list of printed jobs.
        /// </summary>
        public List<CurrentPrintJob> Prints { get; set; }

        public int TotalPrints { get; set; }

        public int TotalFinishedPrints { get; set; }

        public int TotalFailedPrints { get; set; }

        public string TotalPrintTime { get; set; }

        public string LongestPrintJob { get; set; }

        public string FilamentUsed { get; set; }
    }

}