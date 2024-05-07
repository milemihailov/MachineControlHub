using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class ChamberTemperatureService
    {
        public ITemperatures chamber;
        private readonly ISnackbar _snackbar;
        private readonly BackgroundTimer background;

        public int currentChamberTemperature;
        public int setChamberTemperature;
        public int targetChamberTemperature;
        public int PIDChamberCycles;
        public int PIDChamberTemp;

        public ChamberTemperatureService(ISnackbar snackbar, BackgroundTimer background)
        {
            this.background = background;
            chamber = new ChamberTemps((background.ConnectionServiceSerial.printerConnection));
            _snackbar = snackbar;
        }

        public void SetChamberTemperature(int setTemp)
        {
            chamber.SetTemperature(setTemp);
            _snackbar.Add($"Chamber temperature set to {setTemp}°C", Severity.Info);
        }

        public void ParseCurrentChamberTemperature(string input)
        {
            chamber.ParseCurrentTemperature(input);
            currentChamberTemperature = chamber.CurrentTemp;
            setChamberTemperature = chamber.SetTemp;
            targetChamberTemperature = chamber.TargetTemp;
        }

    }
}
