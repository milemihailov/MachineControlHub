namespace ControllingAndManagingApp.Temps
{
    /// <summary>
    /// Represents the Proportional-Integral-Derivative (PID) values used for temperature control.
    /// </summary>
    public class PIDValues
    {
        /// <summary>
        /// Gets or sets the Proportional (P) value for PID control.
        /// </summary>
        public double Proportional { get; set; }

        /// <summary>
        /// Gets or sets the Integral (I) value for PID control.
        /// </summary>
        public double Integral { get; set; }

        /// <summary>
        /// Gets or sets the Derivative (D) value for PID control.
        /// </summary>
        public double Derivative { get; set; }
    }

}