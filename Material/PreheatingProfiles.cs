namespace ControllingAndManagingApp.Material
{
    /// <summary>
    /// Represents preheating profiles for a 3D printer, including hotend temperature, bed temperature, fan speed, and material index.
    /// </summary>
    public class PreheatingProfiles
    {
        /// <summary>
        /// Gets or sets the hotend temperature in degrees Celsius.
        /// </summary>
        public int? HotendTemp { get; set; }

        /// <summary>
        /// Gets or sets the bed temperature in degrees Celsius.
        /// </summary>
        public int? BedTemp { get; set; }

        /// <summary>
        /// Gets or sets the fan speed as a percentage.
        /// </summary>
        public int? FanSpeed { get; set; }

        /// <summary>
        /// Gets or sets the material index.
        /// </summary>
        public int? MaterialIndex { get; set; }

        /// <summary>
        /// Generates a string representation of the hotend temperature for G-code commands.
        /// </summary>
        /// <returns>The string representation of the hotend temperature for G-code commands, or null if not specified.</returns>
        public string HotendTempString()
        {
            if (HotendTemp != null)
            {
                return $"H{HotendTemp}";
            }
            return null;
        }

        /// <summary>
        /// Generates a string representation of the bed temperature for G-code commands.
        /// </summary>
        /// <returns>The string representation of the bed temperature for G-code commands, or null if not specified.</returns>
        public string BedTempString()
        {
            if (BedTemp != null)
            {
                return $"B{BedTemp}";
            }
            return null;
        }

        /// <summary>
        /// Generates a string representation of the fan speed for G-code commands.
        /// </summary>
        /// <returns>The string representation of the fan speed for G-code commands, or null if not specified.</returns>
        public string FanSpeedString()
        {
            if (FanSpeed != null)
            {
                return $"F{FanSpeed}";
            }
            return null;
        }

        /// <summary>
        /// Generates a string representation of the material index for G-code commands.
        /// </summary>
        /// <returns>The string representation of the material index for G-code commands, or null if not specified.</returns>
        public string MaterialIndexString()
        {
            if (MaterialIndex != null)
            {
                return $"S{MaterialIndex}";
            }
            return null;
        }
    }


}