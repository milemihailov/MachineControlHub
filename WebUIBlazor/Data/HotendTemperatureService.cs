using MachineControlHub;
using MachineControlHub.Motion;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public int CurrentHotendTemp { get; set; }
        public int SetHotendTemp { get; set; }
        public int TargetHotendTemp { get; set; }
        public int PIDHotendCycles { get; set; }
        public int PIDHotendTemp { get; set; }
        private DateTime LastChangeTime { get; set; }


        public void SetHotendTemperature(int setTemp, Printer printer)
        {
            printer.HotendTemperatures.SetTemperature(setTemp);

            /// Reset the set temperature to 0
            SetHotendTemp = 0;

            /// Update the target temperature
            TargetHotendTemp = setTemp;

            // Update the timestamp
            LastChangeTime = DateTime.Now;
        }

        public async Task ParseCurrentHotendTemperature(string input, Printer printer)
        {
            await Task.Run(() =>
            {
                printer.HotendTemperatures.ParseCurrentTemperature(input);

                /// Update the current temperature
                CurrentHotendTemp = printer.HotendTemperatures.CurrentTemp;

                /// If the last change was more than 3 seconds ago, update the target temperature
                if ((DateTime.Now - LastChangeTime).TotalSeconds >= 3)
                {
                    TargetHotendTemp = printer.HotendTemperatures.TargetTemp;
                }
            });
        }

        public void ChangeFilament(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildFilamentChangeCommand());
        }

        public void LoadFilament(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildLoadFilamentCommand());
        }

        public void UnloadFilament(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildUnloadFilamentCommand());
        }

        public void SetHotendPIDValues(Printer printer)
        {
            printer.SerialConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(0, PIDHotendTemp, PIDHotendCycles));
        }
    }
}