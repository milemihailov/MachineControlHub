﻿using MachineControlHub.Bed;
using MachineControlHub.Head;
using MachineControlHub.Material;
using MachineControlHub.Motion;
using MachineControlHub.Print;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;

namespace MachineControlHub
{
    /// <summary>
    /// Represents a 3D printer with various properties and components.
    /// </summary>
    public class Printer
    {

        public string Name { get; set; }

        public string Id { get; set; }

        public string LinearUnit { get; set; }

        public string TemperatureUnit { get; set; }

        public bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets the model of the 3D printer.
        /// </summary>
        public string Model { get; set; }

        public int NumberOfExtruders { get; set; }

        public bool HasBinaryFileTransfer { get; set; }

        public bool HasAutoReportTemperature { get; set; }

        public bool HasAutoReportPosition { get; set; }

        public bool HasAutoReportSDStatus { get; set; }

        public bool HasSoftwarePowerControl { get; set; }

        public bool HasEmergencyParser { get; set; }

        public bool HasToggleLights { get; set; }

        public bool HasHostActionCommands { get; set; }

        public bool HasPromptSupport { get; set; }

        public bool HasEEPROM { get; set; }

        public bool HasSDCardSupport { get; set; }

        public bool HasLongFilenameSupport { get; set; }

        public bool HasCustomFirmwareUpload { get; set; }

        public bool HasExtendedM20 { get; set; }

        public bool HasThermalProtection { get; set; }

        public bool HasBabystep { get; set; }

        public bool HasPowerLossRecovery { get; set; }

        public bool MediaAttached { get; set; }

        public bool IsTransferringFile { get; set; }

        /// <summary>
        /// Gets or sets the firmware version installed on the printer.
        /// </summary>
        public string PrinterFirmwareVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the printer has auto bed leveling capability.
        /// </summary>
        public bool HasAutoBedLevel { get; set; }

        public bool BLTTouchHSMode { get; set; }

        public bool AutoBedLevelingEnabled { get; set; }

        public bool HasChamber { get; set; }

        public bool HasHeatedBed { get; set; }

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

        public PrinterHead Head { get; set; }

        /// <summary>
        /// Gets or sets the print head configuration and properties of the printer.
        /// </summary>
        public List<PrinterHead> Extruders { get; set; }

        /// <summary>
        /// Gets or sets the camera configuration and properties of the printer.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the bed temperatures data for the printer.
        /// </summary>
        public BedTemps BedTemperatures { get; set; }

        /// <summary>
        /// Gets or sets the hotend temperatures data for the printer.
        /// </summary>
        public HotendTemps HotendTemperatures { get; set; }

        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets the chamber temperatures data for the printer.
        /// </summary>
        public ChamberTemps ChamberTemperatures { get; set; }

        /// <summary>
        /// Gets or sets the properties of the filament used by the printer.
        /// </summary>
        public FilamentProperties FilamentProperties { get; set; }

        /// <summary>
        /// Gets or sets the preheating profiles for the printer.
        /// </summary>
        public PreheatingProfiles PreheatingProfiles { get; set; }

        /// <summary>
        /// Gets or sets the touchscreen configuration and properties of the printer.
        /// </summary>
        public TouchScreen TouchScreen { get; set; }

        /// <summary>
        /// Gets or sets the print history of the printer.
        /// </summary>
        public PrintHistory PrintHistory { get; set; }

        public PrintService PrintService { get; set; }

        public PIDValues PIDValues { get; set; }

        /// <summary>
        /// Gets or sets the current print job of the printer.
        /// </summary>
        public CurrentPrintJob CurrentPrintJob { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public BedLevelData BedLevelData { get; set; }

        /// <summary>
        /// Gets or sets the motion settings and data for the printer.
        /// </summary>
        public MotionSettingsData MotionSettings { get; set; }

        public StepperDriversData StepperDrivers { get; set; }

        public IPrinterConnection PrinterConnection { get; set; }

        public SerialConnection SerialConnection { get; set; }

        public PrintSchedule PrintSchedule { get; set; }

    }

}