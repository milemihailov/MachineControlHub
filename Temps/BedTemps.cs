namespace ControllingAndManagingApp.Temps
{
    /// <summary>
    /// Represents temperature-related information for the printer's heated bed.
    /// </summary>
    public class BedTemps
    {
        /// <summary>
        /// Gets or sets the PID (Proportional-Integral-Derivative) control values for the bed temperature.
        /// </summary>
        public PIDValues PIDBedValues { get; set; }

        /// <summary>
        /// Gets or sets the current temperature of the heated bed in degrees Celsius.
        /// </summary>
        public int BedCurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the heated bed in degrees Celsius.
        /// </summary>
        public int BedMaxTemp { get; set; }

        /// <summary>
        /// Gets or sets the target temperature set for the heated bed in degrees Celsius.
        /// </summary>
        public int BedSetTemp { get; set; }
    }

}