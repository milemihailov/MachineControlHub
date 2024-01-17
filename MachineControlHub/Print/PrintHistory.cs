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
        public List<PrintJobHistory> Prints { get; set; }
    }

}