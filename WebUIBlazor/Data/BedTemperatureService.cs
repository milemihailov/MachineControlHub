using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class BedTemperatureService
    {
        public ITemperatures bed;
        private readonly ISnackbar _snackbar;
        private readonly BackgroundTimer _background;
        private IPrinterConnection _connection;
        public PIDValues PIDBedValues { get; set; }
        private DateTime _lastChangeTime;

        public int currentBedTemperature;
        public int setBedTemperature;
        public int targetBedTemperature;
        public int PIDBedCycles;
        public int PIDBedTemp;

        public BedTemperatureService(ISnackbar snackbar, BackgroundTimer background) 
        {
            _background = background;
            bed = new BedTemps(background.ConnectionServiceSerial.printerConnection);
            _snackbar = snackbar;
        }

        public void SetBedTemperature(int setTemp) 
        {
            bed.SetTemperature(setTemp);
            setBedTemperature = 0;
            targetBedTemperature = setTemp;
            _lastChangeTime = DateTime.Now; // Update the timestamp
            _snackbar.Add($"Bed temperature set to {setTemp}°C", Severity.Info);
        }

        public async Task ParseCurrentBedTemperature(string input)
        {
            await Task.Run(() => {
                bed.ParseCurrentTemperature(input);
                currentBedTemperature = bed.CurrentTemp;
                if ((DateTime.Now - _lastChangeTime).TotalSeconds >= 3)
                {
                    targetBedTemperature = bed.TargetTemp;
                }
            } );
            
        }

        public void SetBedPIDValues()
        {
            _background.ConnectionServiceSerial.Write(CommandMethods.BuildPIDAutoTuneCommand(-1, PIDBedTemp, PIDBedCycles, true));
            _snackbar.Add($"Setting PID Autotune for BED {PIDBedTemp}°C and {PIDBedCycles} cycles!", Severity.Info);
        }
    }
}
