namespace ControllingAndManagingApp.Print
{
    /// <summary>
    /// Represents a 3D printing job.
    /// </summary>
    public class Print
    {
        /// <summary>
        /// Gets or sets the name of the printed file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the settings extracted from the printed file.
        /// </summary>
        public string ExtractedSettingsFromPrintedFile { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the print job.
        /// </summary>
        public DateTime TimeOfPrint { get; set; }
    }

}