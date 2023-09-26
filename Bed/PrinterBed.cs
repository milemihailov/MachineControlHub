namespace ControllingAndManagingApp.Bed
{
    /// <summary>
    /// Represents the properties of the printer's bed, such as its size and heating capability.
    /// </summary>
    public class PrinterBed
    {
        /// <summary>
        /// Gets or sets a value indicating whether the printer's bed is heated.
        /// </summary>
        public bool HeatedBed { get; set; }

        /// <summary>
        /// Gets or sets the shape of the printer's bed, e.g., rectangular or circular.
        /// </summary>
        public string ShapeOfBed { get; set; }

        /// <summary>
        /// Gets or sets the width (X-size) of the printer's bed in millimeters.
        /// </summary>
        public double XSize { get; set; }

        /// <summary>
        /// Gets or sets the length (Y-size) of the printer's bed in millimeters.
        /// </summary>
        public double YSize { get; set; }
    }
}