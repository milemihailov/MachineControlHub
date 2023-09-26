namespace ControllingAndManagingApp.Temps
{
    /// <summary>
    /// Represents temperature-related information for the printer's hotend.
    /// </summary>
    public class HotendTemps
    {
        /// <summary>
        /// Gets or sets the PID (Proportional-Integral-Derivative) values for controlling the hotend temperature.
        /// </summary>
        public PIDValues PIDHotendValues { get; set; }

        /// <summary>
        /// Gets or sets the current temperature of the hotend in degrees Celsius.
        /// </summary>
        public int HotendCurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the hotend in degrees Celsius.
        /// </summary>
        public int HotendMaxTemp { get; set; }

        /// <summary>
        /// Gets or sets the target temperature set for the hotend in degrees Celsius.
        /// </summary>
        public int HotendSetTemp { get; set; }
    }

}
