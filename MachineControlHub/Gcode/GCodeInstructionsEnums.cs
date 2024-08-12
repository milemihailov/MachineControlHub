namespace MachineControlHub.Gcode
{
    public class GCodeInstructionsEnums
    {
        /// <summary>
        /// Represents a set of G-code commands for controlling 3D printer movements and operations.
        /// </summary>
        public enum GCommands
        {
            /// <summary>
            /// Command for rapid linear moves, typically used for non-printing toolhead positioning.
            /// </summary>
            RapidLinearMove,

            /// <summary>
            /// Command for linear moves, used for printing and positioning.
            /// </summary>
            LinearMove,

            /// <summary>
            /// Command for clockwise arc moves.
            /// </summary>
            ClockwiseArcMove,

            /// <summary>
            /// Command for counterclockwise arc moves.
            /// </summary>
            CounterClockwiseArcMove,

            /// <summary>
            /// Command to retract filament according to settings.
            /// </summary>
            RetractFilamentAccordingToSettings = 10,

            /// <summary>
            /// Command to recover filament according to settings.
            /// </summary>
            RetractRecoverFilamentAccordingToSettings,

            /// <summary>
            /// Command to clean the nozzle.
            /// </summary>
            CleanTheNozzle,

            /// <summary>
            /// Command for mesh validation pattern.
            /// </summary>
            MeshValidationPattern = 26,

            /// <summary>
            /// Command to park the toolhead.
            /// </summary>
            ParkToolHead,

            /// <summary>
            /// Command to home all axes.
            /// </summary>
            HomeAllAxes,

            /// <summary>
            /// Command for auto bed leveling.
            /// </summary>
            AutoBedLeveling,

            /// <summary>
            /// Command for a single Z-probe operation.
            /// </summary>
            SingleZProbe,

            ZSteppersAutoAlignment = 34,

            /// <summary>
            /// Command to save the current position.
            /// </summary>
            SaveCurrentPossition = 60,

            /// <summary>
            /// Command to return to a saved position.
            /// </summary>
            ReturnToSavedPosition,

            /// <summary>
            /// Command to cancel the bed leveling procedure.
            /// </summary>
            CancelTheBedLevelingProcedure = 80,

            /// <summary>
            /// Command for absolute positioning.
            /// </summary>
            AbsolutePositioning = 90,

            /// <summary>
            /// Command for relative positioning.
            /// </summary>
            RelativePositioning,

            /// <summary>
            /// Command to set positions.
            /// </summary>
            SetPositions,
        }


        /// <summary>
        /// Represents a set of M-code commands used in 3D printing and CNC machining.
        /// </summary>
        public enum MCommands
        {
            /// <summary>
            /// Indicates the end of a program or script.
            /// </summary>
            EndOfProgram = 2,

            /// <summary>
            /// Disables the stepper motors.
            /// </summary>
            DisableSteppers = 18,

            /// <summary>
            /// Lists files on the SD card.
            /// </summary>
            ListSdCard = 20,

            /// <summary>
            /// Initializes the SD card.
            /// </summary>
            InitSdCard,

            /// <summary>
            /// Releases the SD card.
            /// </summary>
            ReleaseSdCard,

            /// <summary>
            /// Selects a file on the SD card.
            /// </summary>
            SelectSdFile,

            /// <summary>
            /// Starts printing from the SD card.
            /// </summary>
            StartPrint,

            /// <summary>
            /// Pauses the SD card print.
            /// </summary>
            PauseSdPrint,

            /// <summary>
            /// Sets the SD card position.
            /// </summary>
            SetSdPosition,

            /// <summary>
            /// Reports the status of the SD card print.
            /// </summary>
            ReportSdPrintStatus,

            /// <summary>
            /// Starts writing to the SD card.
            /// </summary>
            StartSdWrite,

            /// <summary>
            /// Stops writing to the SD card.
            /// </summary>
            StopSdWrite,

            /// <summary>
            /// Deletes a file from the SD card.
            /// </summary>
            DeleteSdFile,

            /// <summary>
            /// Reports the print time.
            /// </summary>
            PrintTime,

            /// <summary>
            /// Selects and starts a specific action.
            /// </summary>
            SelectAndStart,

            /// <summary>
            /// Initiates a probe repeatability test.
            /// </summary>
            ProbeRepeatabilityTest = 48,

            /// <summary>
            /// Sets the print progress.
            /// </summary>
            SetPrintProgress = 73,

            /// <summary>
            /// Starts the print job timer.
            /// </summary>
            StartPrintJobTimer = 75,

            /// <summary>
            /// Pauses the print job timer.
            /// </summary>
            PausePrintJobTimer,

            /// <summary>
            /// Stops the print job timer.
            /// </summary>
            StopPrintJobTimer,

            /// <summary>
            /// Reports print job statistics.
            /// </summary>
            PrintJobStats,

            /// <summary>
            /// Powers on the device.
            /// </summary>
            PowerOn = 80,

            /// <summary>
            /// Powers off the device.
            /// </summary>
            PowerOff,

            /// <summary>
            /// Sets the extruder to absolute mode.
            /// </summary>
            EAbsolute,

            /// <summary>
            /// Sets the extruder to relative mode.
            /// </summary>
            ERelative,

            /// <summary>
            /// Sets axis steps per unit.
            /// </summary>
            SetAxisStepsPerUnit = 92,

            /// <summary>
            /// Retrieves free memory information.
            /// </summary>
            FreeMemory = 100,

            /// <summary>
            /// Sets the hotend temperature.
            /// </summary>
            SetHotendTemperature = 104,

            /// <summary>
            /// Reports temperatures.
            /// </summary>
            ReportTemperatures,

            /// <summary>
            /// Sets the fan speed.
            /// </summary>
            SetFanSpeed,

            /// <summary>
            /// Turns off the fan.
            /// </summary>
            FanOff,

            /// <summary>
            /// Breaks and continues execution.
            /// </summary>
            BreakAndContinue,

            /// <summary>
            /// Waits for the hotend temperature.
            /// </summary>
            WaitForHotendTemperature,

            /// <summary>
            /// Fully shuts down the device.
            /// </summary>
            FullShutDown = 112,

            /// <summary>
            /// Sends a keep-alive signal to the host.
            /// </summary>
            HostKeepAlive,

            /// <summary>
            /// Retrieves the current position.
            /// </summary>
            GetCurrentPosition,

            /// <summary>
            /// Retrieves firmware information.
            /// </summary>
            FirmwareInfo,

            /// <summary>
            /// Sets the LCD status.
            /// </summary>
            SetLCDStatus = 117,

            /// <summary>
            /// Prints a message to the serial port.
            /// </summary>
            SerialPrint,

            /// <summary>
            /// Reports endstop states.
            /// </summary>
            EndstopStates,

            /// <summary>
            /// Enables endstops.
            /// </summary>
            EnableEndstops,

            /// <summary>
            /// Disables endstops.
            /// </summary>
            DisableEndstops,

            /// <summary>
            /// Sends TMC debugging commands.
            /// </summary>
            TMCDebug,

            /// <summary>
            /// Parks the tool head.
            /// </summary>
            ParkHead = 125,

            /// <summary>
            /// Sets the bed temperature.
            /// </summary>
            SetBedTemperature = 140,

            /// <summary>
            /// Sets the chamber temperature.
            /// </summary>
            SetChamberTemperature,

            /// <summary>
            /// Sets a material preset.
            /// </summary>
            SetMaterialPreset = 145,

            /// <summary>
            /// Sets the temperature units.
            /// </summary>
            SetTemperatureUnits = 149,

            /// <summary>
            /// Sets RGB colors.
            /// </summary>
            SetRGBColors,

            /// <summary>
            /// Enables position auto-reporting.
            /// </summary>
            PositionAutoReport = 154,

            /// <summary>
            /// Enables temperature auto-reporting.
            /// </summary>
            TemperatureAutoReport,

            /// <summary>
            /// Waits for the bed temperature.
            /// </summary>
            WaitForBedTemperature = 190,

            /// <summary>
            /// Waits for the chamber temperature.
            /// </summary>
            WaitForChamberTemperature,

            /// <summary>
            /// Waits for the probe temperature.
            /// </summary>
            WaitForProbeTemperature,

            /// <summary>
            /// Sets the filament diameter.
            /// </summary>
            SetFilamentDiameter = 200,

            /// <summary>
            /// Prints travel move limits.
            /// </summary>
            PrintTravelMoveLimits,

            /// <summary>
            /// Sets the maximum feedrates.
            /// </summary>
            SetMaxFeedrates = 203,

            /// <summary>
            /// Sets the starting acceleration.
            /// </summary>
            SetStartingAcceleration,

            /// <summary>
            /// Sets advanced settings.
            /// </summary>
            SetAdvancedSettings,

            /// <summary>
            /// Sets home offsets.
            /// </summary>
            SetHomeOffsets,

            /// <summary>
            /// Sets firmware retraction.
            /// </summary>
            SetFirmwareRetraction,

            /// <summary>
            /// Recovers from firmware error.
            /// </summary>
            FirmwareRecover,

            /// <summary>
            /// Sets auto-retract behavior.
            /// </summary>
            SetAutoRetract,

            SoftwareEndstops = 211,

            /// <summary>
            /// Sets hotend offsets.
            /// </summary>
            SetHotendOffset = 218,

            /// <summary>
            /// Sets feedrate percentage.
            /// </summary>
            SetFeedratePercentage = 220,

            /// <summary>
            /// Sets flow percentage.
            /// </summary>
            SetFlowPercentage,

            /// <summary>
            /// Triggers a camera action.
            /// </summary>
            TriggerCamera = 240,

            /// <summary>
            /// Sends an I2C message.
            /// </summary>
            I2CSend = 260,

            /// <summary>
            /// Requests an I2C message.
            /// </summary>
            I2CRequest,

            /// <summary>
            /// Performs baby stepping.
            /// </summary>
            BabyStep = 290,

            /// <summary>
            /// Plays a tone.
            /// </summary>
            PlayTone = 300,

            /// <summary>
            /// Sets the hotend PID values.
            /// </summary>
            SetHotendPID,

            /// <summary>
            /// Initiates a cold extrusion.
            /// </summary>
            ColdExtrude,

            /// <summary>
            /// Performs PID auto-tuning.
            /// </summary>
            PIDAutoTune,

            /// <summary>
            /// Sets the bed PID values.
            /// </summary>
            SetBedPID,

            /// <summary>
            /// Specifies user thermistor parameters.
            /// </summary>
            UserThermistorParameters,

            /// <summary>
            /// Deploys the probe.
            /// </summary>
            DeployProbe = 401,

            /// <summary>
            /// Stows the probe.
            /// </summary>
            StowProbe,

            /// <summary>
            /// Reports bed leveling state.
            /// </summary>
            BedLevelingState = 420,

            /// <summary>
            /// Sets mesh values.
            /// </summary>
            SetMeshValue,

            /// <summary>
            /// Saves settings to EEPROM.
            /// </summary>
            SaveSettings = 500,

            /// <summary>
            /// Restores settings from EEPROM.
            /// </summary>
            RestoreSettings,

            /// <summary>
            /// Performs a factory reset.
            /// </summary>
            FactoryReset,

            /// <summary>
            /// Reports current settings.
            /// </summary>
            ReportSettings,

            /// <summary>
            /// Abort current print.
            /// </summary>
            AbortSDPrint = 524,

            /// <summary>
            /// Sets TMC stepping mode.
            /// </summary>
            SetTMCSteppingMode = 569,

            /// <summary>
            /// Sets the serial baud rate.
            /// </summary>
            SerialBaudRate = 575,

            /// <summary>
            /// Initiates filament change.
            /// </summary>
            FilamentChange = 600,

            /// <summary>
            /// Loads filament code.
            /// </summary>
            LoadFilament = 701,

            /// <summary>
            /// Unloads filament code.
            /// </summary>
            UnloadFilament,

            /// <summary>
            /// Sets XYZ probe offsets.
            /// </summary>
            XYZProbeOffset = 851,

            /// <summary>
            /// Sets stepper motor current.
            /// </summary>
            StepperMotorCurrent = 906,

            TMCBumpSensitivity = 914,
        }

    }
}