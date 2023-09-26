namespace ControllingAndManagingApp.Temps
{
    /// <summary>
    /// Represents temperature-related information for the printer's chamber.
    /// </summary>
    public class ChamberTemps
    {
        /// <summary>
        /// Gets or sets the current temperature of the chamber in degrees Celsius.
        /// </summary>
        public int ChamberCurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the chamber in degrees Celsius.
        /// </summary>
        public int ChamberMaxTemp { get; set; }

        /// <summary>
        /// Gets or sets the target temperature set for the chamber in degrees Celsius.
        /// </summary>
        public int ChamberSetTemp { get; set; }
    }

}
