namespace MachineControlHub.Bed
{

    /// <summary>
    /// Represents the properties of the printer's bed, such as its size and heating capability.
    /// </summary>
    public class PrinterBed
    {
        public enum BedShapes
        {
            Rectangular,
            Circular,
            Custom
        }

        /// <summary>
        /// Gets or sets a value indicating whether the printer's bed is heated.
        /// </summary>
        public bool HeatedBed { get; set; }

        // Private field to store the selected shape
        private BedShapes shapeOfBed;

        /// <summary>
        /// Gets or sets the shape of the printer's bed.
        /// </summary>
        public BedShapes ShapeOfBed
        {
            get { return shapeOfBed; }
            set
            {
                shapeOfBed = value;
            }
        }

        /// <summary>
        /// Gets or sets the width (X-size) of the printer's bed in millimeters.
        /// </summary>
        public int XSize { get; set; }

        /// <summary>
        /// Gets or sets the length (Y-size) of the printer's bed in millimeters.
        /// </summary>
        public int YSize { get; set; }
    }
}