using MachineControlHub;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using MudBlazor;
using WebUI.Pages;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public PrinterManagerService Printer { get; set; }

        public int CurrentHotendTemp { get; set;}
        public int SetHotendTemp { get; set;}
        public int TargetHotendTemp { get; set;}
        public int PIDHotendCycles { get; set;}
        public int PIDHotendTemp { get; set;}
        private DateTime LastChangeTime { get; set;}

        public HotendTemperatureService(PrinterManagerService Printer)
        {
            this.Printer = Printer;
        }

        public void SetHotendTemperature(int setTemp)
        {
            Printer.ActivePrinter.HotendTemperatures.SetTemperature(setTemp);

            /// Reset the set temperature to 0
            SetHotendTemp = 0;

            /// Update the target temperature
            TargetHotendTemp = setTemp;

            // Update the timestamp
            LastChangeTime = DateTime.Now;
        }

        public async Task ParseCurrentHotendTemperature(string input)
        {
            await Task.Run(() =>
            {
                Printer.ActivePrinter.HotendTemperatures.ParseCurrentTemperature(input);

                /// Update the current temperature
                CurrentHotendTemp = Printer.ActivePrinter.HotendTemperatures.CurrentTemp;

                /// If the last change was more than 3 seconds ago, update the target temperature
                if ((DateTime.Now - LastChangeTime).TotalSeconds >= 3)
                {
                    TargetHotendTemp = Printer.ActivePrinter.HotendTemperatures.TargetTemp;
                }
            } );
        }

        public void ChangeFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildFilamentChangeCommand());
        }
        
        public void LoadFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildLoadFilamentCommand());
        }

        public void UnloadFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildUnloadFilamentCommand());
        }

        public void SetHotendPIDValues()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(0, PIDHotendTemp, PIDHotendCycles));
        }
    }
}