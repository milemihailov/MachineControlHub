using MachineControlHub.Motion;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public ITemperatures hotend ;
        private readonly ISnackbar _snackbar;
        private readonly BackgroundTimer _background;

        public int currentHotendTemperature;
        public int setHotendTemperature;
        public int targetHotendTemperature;
        public int PIDHotendCycles;
        public int PIDHotendTemp;
        private DateTime _lastChangeTime;

        public HotendTemperatureService(ISnackbar snackbar, BackgroundTimer background)
        {
            this._background = background;
            hotend = new HotendTemps(background.ConnectionServiceSerial.printerConnection);
            _snackbar = snackbar;
        }
        public void SetHotendTemperature(int setTemp)
        {
            hotend.SetTemperature(setTemp);
            setHotendTemperature = 0;
            targetHotendTemperature = setTemp;
            _lastChangeTime = DateTime.Now; // Update the timestamp
            _snackbar.Add($"Hotend temperature set to {setTemp}°C", Severity.Info);
        }
        public async Task ParseCurrentHotendTemperature(string input)
        {
            await Task.Run(() =>
            {
                hotend.ParseCurrentTemperature(input);
                currentHotendTemperature = hotend.CurrentTemp;

                if ((DateTime.Now - _lastChangeTime).TotalSeconds >= 3)
                {
                    targetHotendTemperature = hotend.TargetTemp;
                }
            } );
        }

        public void ChangeFilament()
        {
            _background.ConnectionServiceSerial.Write(CommandMethods.BuildFilamentChangeCommand());
            _snackbar.Add("Filament Change Command Sent", Severity.Info);
        }
        
        public void LoadFilament()
        {
            _background.ConnectionServiceSerial.Write(CommandMethods.BuildLoadFilamentCommand());
            _snackbar.Add("Filament Load Command Sent", Severity.Info);
        }

        public void UnloadFilament()
        {
            _background.ConnectionServiceSerial.Write(CommandMethods.BuildUnloadFilamentCommand());
            _snackbar.Add("Filament Unload Command Sent", Severity.Info);
        }

        public void SetHotendPIDValues()
        {
            _background.ConnectionServiceSerial.Write(CommandMethods.BuildPIDAutoTuneCommand(0, PIDHotendTemp, PIDHotendCycles, true));
            _snackbar.Add($"Setting PID Autotune for HOTEND {PIDHotendTemp}°C and {PIDHotendCycles} cycles!", Severity.Info);
        }
    }
}
