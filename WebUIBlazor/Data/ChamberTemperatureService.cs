using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class ChamberTemperatureService
    {
        public ITemperatures chamber;
        private readonly ISnackbar _snackbar;
        private readonly PortConnectionManagerService _portConnectionManager;

        public int currentChamberTemperature;
        public int setChamberTemperature;
        public int targetChamberTemperature;
        public int PIDChamberCycles;
        public int PIDChamberTemp;

        public ChamberTemperatureService(ISnackbar snackbar, PortConnectionManagerService portConnectionManager)
        {
            this._portConnectionManager = portConnectionManager;
            chamber = new ChamberTemps((portConnectionManager.connection.ConnectionServiceSerial.printerConnection));
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
