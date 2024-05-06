using MachineControlHub.Motion;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class BedTemperatureService
    {
        public ITemperatures bed;
        private readonly ISnackbar _snackbar;

        public int currentBedTemperature;
        public int setBedTemperature;
        public int targetBedTemperature;
        public int PIDBedCycles;
        public int PIDBedTemp;

        public BedTemperatureService(ISnackbar snackbar) 
        {
            bed = new BedTemps(ConnectionServiceSerial.printerConnection);
            _snackbar = snackbar;
        }

        public void SetBedTemperature(int setTemp) 
        {
            bed.SetTemperature(setTemp);
            _snackbar.Add($"Bed temperature set to {setTemp}°C", Severity.Info);
        }

        public void ParseCurrentBedTemperature(string input)
        {
            bed.ParseCurrentTemperature(input);
            currentBedTemperature = bed.CurrentTemp;
            setBedTemperature = bed.SetTemp;
            targetBedTemperature = bed.TargetTemp;
        }

        public void SetBedPIDValues()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(0, PIDBedTemp, PIDBedCycles, true));
            _snackbar.Add($"Setting PID Autotune for BED {PIDBedTemp}°C and {PIDBedCycles} cycles!", Severity.Info);
        }
    }
}
