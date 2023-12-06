using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Temps;

namespace MachineControlHub
{
    /// <summary>
    /// Represents a 3D printer with various properties and components.
    /// </summary>
    public class Printer
    {
        /// <summary>
        /// Gets or sets the model of the 3D printer.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the firmware version installed on the printer.
        /// </summary>
        public string PrinterFirmwareVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the printer has auto bed leveling capability.
        /// </summary>
        public bool HasAutoBedLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the printer has a filament runout sensor.
        /// </summary>
        public bool HasFilamentRunoutSensor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the printer is connected to a network.
        /// </summary>
        public bool NetworkConnection { get; set; }

        /// <summary>
        /// Gets or sets the maximum build volume in the Z-axis of the printer.
        /// </summary>
        public int ZMaxBuildVolume { get; set; }

        /// <summary>
        /// Gets or sets the bed configuration and properties of the printer.
        /// </summary>
        public PrinterBed Bed { get; set; }

        /// <summary>
        /// Gets or sets the print head configuration and properties of the printer.
        /// </summary>
        public PrinterHead Head { get; set; }

        /// <summary>
        /// Gets or sets the bed temperatures data for the printer.
        /// </summary>
        public BedTemps BedTemperatures { get; set; }

        /// <summary>
        /// Gets or sets the hotend temperatures data for the printer.
        /// </summary>
        public HotendTemps HotendTemperatures { get; set; }

        /// <summary>
        /// Gets or sets the chamber temperatures data for the printer.
        /// </summary>
        public ChamberTemps ChamberTemperatures { get; set; }

        /// <summary>
        /// Gets or sets the properties of the filament used by the printer.
        /// </summary>
        public FilamentProperties FilamentUsed { get; set; }

        /// <summary>
        /// Gets or sets the touchscreen configuration and properties of the printer.
        /// </summary>
        public TouchScreen TouchScreen { get; set; }

        /// <summary>
        /// Gets or sets the motion settings and data for the printer.
        /// </summary>
        public MotionSettingsData MotionSettings { get; set; }
    }

}