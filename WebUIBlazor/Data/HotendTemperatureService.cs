using MachineControlHub.Motion;
using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public ITemperatures hotend ;
        private readonly ISnackbar _snackbar;
        private readonly BackgroundTimer background;

        public int currentHotendTemperature;
        public int setHotendTemperature;
        public int targetHotendTemperature;
        public int PIDHotendCycles;
        public int PIDHotendTemp;

        public HotendTemperatureService(ISnackbar snackbar, BackgroundTimer background)
        {
            this.background = background;
            hotend = new HotendTemps(background.ConnectionServiceSerial.printerConnection);
            _snackbar = snackbar;
        }
        public void SetHotendTemperature(int setTemp)
        {
            hotend.SetTemperature(setTemp);
            _snackbar.Add($"Hotend temperature set to {setTemp}°C", Severity.Info);
        }
        public void ParseCurrentHotendTemperature(string input)
        {
            hotend.ParseCurrentTemperature(input);
            currentHotendTemperature = hotend.CurrentTemp;
            setHotendTemperature = hotend.SetTemp;
            targetHotendTemperature = hotend.TargetTemp;
        }

        public void ChangeFilament()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildFilamentChangeCommand());
            _snackbar.Add("Filament Change Command Sent", Severity.Info);
        }
        
        public void LoadFilament()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildLoadFilamentCommand());
            _snackbar.Add("Filament Load Command Sent", Severity.Info);
        }

        public void UnloadFilament()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildUnloadFilamentCommand());
            _snackbar.Add("Filament Unload Command Sent", Severity.Info);
        }

        public void SetHotendPIDValues()
        {
            background.ConnectionServiceSerial.Write(CommandMethods.BuildPIDAutoTuneCommand(-1, PIDHotendTemp, PIDHotendCycles, true));
            _snackbar.Add($"Setting PID Autotune for HOTEND {PIDHotendTemp}°C and {PIDHotendCycles} cycles!", Severity.Info);
        }
    }
}
