namespace MachineControlHub.Head
{
    /// <summary>
    /// Represents the printer's extruder head, including various properties and components.
    /// </summary>
    public class PrinterHead
    {
        /// <summary>
        /// Gets or sets a value indicating whether a part cooling fan is present on the extruder head.
        /// </summary>
        public bool HasCoolingFan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a probe (e.g., auto-leveling probe) is present on the extruder head.
        /// </summary>
        public bool ProbePresent { get; set; }

        /// <summary>
        /// Gets or sets the diameter of the extruder nozzle in millimeters.
        /// </summary>
        public double NozzleDiameter { get; set; }

        /// <summary>
        /// Gets or sets the material used for the extruder nozzle.
        /// </summary>
        public string NozzleMaterial { get; set; }
    }

}