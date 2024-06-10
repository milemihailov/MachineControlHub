namespace MachineControlHub.Material
{
    /// <summary>
    /// Represents properties and characteristics of a 3D printer filament.
    /// </summary>
    public class FilamentProperties
    {
        /// <summary>
        /// Gets or sets the diameter of the filament in millimeters.
        /// </summary>
        public double FilamentDiameter { get; set; }

        /// <summary>
        /// Gets or sets the mass of the filament in grams.
        /// </summary>
        public double FilamentMass { get; set; }

        /// <summary>
        /// Gets or sets the density of the filament material in grams per cubic centimeter (g/cm³).
        /// </summary>
        public double FilamentDensity { get; set; }

        /// <summary>
        /// Gets or sets the price per kilogram (kg) of the filament.
        /// </summary>
        public double FilamentPricePerKg { get; set; }

        /// <summary>
        /// Gets or sets the length of the filament in meters.
        /// </summary>
        public double FilamentLength { get; set; }

    }

}
