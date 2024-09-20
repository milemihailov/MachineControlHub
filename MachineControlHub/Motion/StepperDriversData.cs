namespace MachineControlHub.Motion
{
    public class StepperDriversData
    {

        public Dictionary<string, double?> GetDriverCurrents()
        {
            return new Dictionary<string, double?>
            {
                { "X", XDriverCurrent },
                { "Y", YDriverCurrent },
                { "Z", ZDriverCurrent },
                { "I1 Z", Z2DriverCurrent },
                { "I2 Z", Z3DriverCurrent },
                { "T1 E", EDriverCurrent },
                { "T2 E", E2DriverCurrent },
                { "T3 E", E3DriverCurrent },
                { "T4 E", E4DriverCurrent },
                { "T5 E", E5DriverCurrent },
            };
        }

        public Dictionary<string, string> GetDriverSteppingMode()
        {
            return new Dictionary<string, string>
            {
                { "X", XDriverSteppingMode },
                { "Y", YDriverSteppingMode },
                { "Z", ZDriverSteppingMode },
                { "I1 Z", Z2DriverSteppingMode },
                { "I2 Z", Z3DriverSteppingMode },
                { "T1 E", EDriverSteppingMode },
                { "T2 E", E2DriverSteppingMode },
                { "T3 E", E3DriverSteppingMode },
                { "T4 E", E4DriverSteppingMode },
                { "T5 E", E5DriverSteppingMode },
            };
        }


        public double? XDriverCurrent { get; set; }
        public double? YDriverCurrent { get; set; }
        public double? ZDriverCurrent { get; set; }
        public double? Z2DriverCurrent { get; set; }
        public double? Z3DriverCurrent { get; set; }
        public double? EDriverCurrent { get; set; }
        public double? E2DriverCurrent { get; set; }
        public double? E3DriverCurrent { get; set; }
        public double? E4DriverCurrent { get; set; }
        public double? E5DriverCurrent { get; set; }

        public string XDriverSteppingMode { get; set; }
        public string YDriverSteppingMode { get; set; }
        public string ZDriverSteppingMode { get; set; }
        public string Z2DriverSteppingMode { get; set; }
        public string Z3DriverSteppingMode { get; set; }
        public string EDriverSteppingMode { get; set; }
        public string E2DriverSteppingMode { get; set; }
        public string E3DriverSteppingMode { get; set; }
        public string E4DriverSteppingMode { get; set; }
        public string E5DriverSteppingMode { get; set; }

        public double? XDriverMicrosteps { get; set; }
        public double? YDriverMicrosteps { get; set; }
        public double? ZDriverMicrosteps { get; set; }
        public double? Z2DriverMicrosteps { get; set; }
        public double? Z3DriverMicrosteps { get; set; }
        public double? EDriverMicrosteps { get; set; }
        public double? E2DriverMicrosteps { get; set; }
        public double? E3DriverMicrosteps { get; set; }
        public double? E4DriverMicrosteps { get; set; }
        public double? E5DriverMicrosteps { get; set; }

        public double? XStallGuardTreshold { get; set; }
        public double? YStallGuardTreshold { get; set; }
        public double? ZStallGuardTreshold { get; set; }

    }
}
