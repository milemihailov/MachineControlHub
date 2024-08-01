using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class BedTemperatureService
    {
        public PrinterManagerService Printer { get; set; }

        private DateTime _lastChangeTime;

        public int currentBedTemperature;
        public int setBedTemperature;
        public int targetBedTemperature;
        public int PIDBedCycles;
        public int PIDBedTemp;

        public BedTemperatureService(PrinterManagerService Printer) 
        {
            this.Printer = Printer;
        }

        public void SetBedTemperature(int setTemp) 
        {
            Printer.ActivePrinter.BedTemperatures.SetTemperature(setTemp);
            setBedTemperature = 0;
            targetBedTemperature = setTemp;
            _lastChangeTime = DateTime.Now; // Update the timestamp
            //_snackbar.Add($"Bed temperature set to {setTemp}°C", Severity.Info);
        }

        public async Task ParseCurrentBedTemperature(string input)
        {
            await Task.Run(() => {
                Printer.ActivePrinter.BedTemperatures.ParseCurrentTemperature(input);
                currentBedTemperature = Printer.ActivePrinter.BedTemperatures.CurrentTemp;
                if ((DateTime.Now - _lastChangeTime).TotalSeconds >= 3)
                {
                    targetBedTemperature = Printer.ActivePrinter.BedTemperatures.TargetTemp;
                }
            } );
            
        }

        public void SetBedPIDValues()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(-1, PIDBedTemp, PIDBedCycles, true));
            //_snackbar.Add($"Setting PID Autotune for BED {PIDBedTemp}°C and {PIDBedCycles} cycles!", Severity.Info);
        }
    }
}
