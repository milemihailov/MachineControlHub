using MachineControlHub;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class BedTemperatureService
    {

        public int CurrentBedTemp { get; set; }
        public int SetBedTemp { get; set; }
        public int TargetBedTemp { get; set; }
        public int PIDBedCycles { get; set; }
        public int PIDBedTemp { get; set; }
        private DateTime LastChangeTime { get; set; }

        public void SetBedTemperature(int setTemp, Printer printer) 
        {
            printer.BedTemperatures.SetTemperature(setTemp);

            // Set the target temperature to the new set temperature
            SetBedTemp = 0;

            // Set the target temperature to the new set temperature
            TargetBedTemp = setTemp;

            // Update the timestamp
            LastChangeTime = DateTime.Now;
        }

        public async Task ParseCurrentBedTemperature(string input, Printer printer)
        {
            await Task.Run(() => {
                printer.BedTemperatures.ParseCurrentTemperature(input);

                // Update the current bed temperature
                CurrentBedTemp = printer.BedTemperatures.CurrentTemp;

                // Update the target bed temperature if it has been 3 seconds since the last change
                if ((DateTime.Now - LastChangeTime).TotalSeconds >= 3)
                {
                    TargetBedTemp = printer.BedTemperatures.TargetTemp;
                }
            } );
            
        }

        public void SetBedPIDValues(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(-1, PIDBedTemp, PIDBedCycles));
        }
    }
}
