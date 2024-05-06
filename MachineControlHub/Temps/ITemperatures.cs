using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineControlHub.Temps
{
    public interface ITemperatures
    {
        /// <summary>
        /// Gets or sets the current temperature of the hotend in degrees Celsius.
        /// </summary>
        public int CurrentTemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable temperature for the hotend in degrees Celsius.
        /// </summary>
        public int MaxTemp { get; set; }

        /// <summary>
        /// Gets or sets the target temperature set for the hotend in degrees Celsius.
        /// </summary>
        public int SetTemp { get; set; }

        public int TargetTemp { get; set; }
        void ParseCurrentTemperature(string input);

        void SetTemperature(int setTemp);
    }
}
