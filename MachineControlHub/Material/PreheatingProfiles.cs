namespace MachineControlHub.Material
{
    /// <summary>
    /// Represents preheating profiles for a 3D printer, including hotend temperature, bed temperature, fan speed, and material index.
    /// </summary>
    public class PreheatingProfiles
    {
        public enum Prefixes
        {
            Hotend = 'H',
            Bed = 'B',
            Fan = 'F',
            MaterialIndex = 'S'
        }
        public string MaterialName { get; set; }
        /// <summary>
        /// Gets or sets the hotend temperature in degrees Celsius.
        /// </summary>
        public int HotendTemp { get; set; }

        /// <summary>
        /// Gets or sets the bed temperature in degrees Celsius.
        /// </summary>
        public int BedTemp { get; set; }

        /// <summary>
        /// Gets or sets the fan speed as a percentage.
        /// </summary>
        public int FanSpeed { get; set; }

        /// <summary>
        /// Gets or sets the material index.
        /// </summary>
        public int MaterialIndex { get; set; }


        /// <summary>
        /// Constructs a string by concatenating a prefix and a value.
        /// </summary>
        /// <param name="prefix">The prefix to prepend to the value.</param>
        /// <param name="value">The value to be concatenated with the prefix.</param>
        /// <returns>
        /// A string that consists of the prefix followed by the value. Returns null if the value is null.
        /// </returns>
        public string StringWithPrefixAndValue(Prefixes prefix, object value)
        {
            if (value != null)
            {
                return $"{(char)prefix}{value}";
            }
            return null;
        }
    }


}