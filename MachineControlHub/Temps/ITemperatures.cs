namespace MachineControlHub.Temps
{
    public interface ITemperatures
    {
        PIDValues PIDValues { get; set; }
        /// <summary>
        /// Gets or sets the current temperature of the hotend in degrees Celsius.
        /// </summary>
        public int CurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the hotend in degrees Celsius.
        /// </summary>
        public int MaxTemp { get; set; }

        public int TargetTemp { get; set; }

        void ParseCurrentTemperature(string input);

        void SetTemperature(int setTemp);
    }
}
