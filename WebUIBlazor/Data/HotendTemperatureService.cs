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
        public ITemperatures hotend;
        public PrinterManagerService Printer { get; set; }

        public PIDValues PIDHotendValues { get; set; }

        public int currentHotendTemperature;
        public int setHotendTemperature;
        public int targetHotendTemperature;
        public int PIDHotendCycles;
        public int PIDHotendTemp;
        private DateTime _lastChangeTime;

        public HotendTemperatureService(PrinterManagerService Printer)
        {
            this.Printer = Printer;
            hotend = new HotendTemps(Printer.ActivePrinter.SerialConnection);
            PIDHotendValues = hotend.PIDValues;
            Printer.ActivePrinter.PIDValues = hotend.PIDValues;
        }

        public void SetHotendTemperature(int setTemp)
        {
            Printer.ActivePrinter.HotendTemperatures.SetTemperature(setTemp);
            setHotendTemperature = 0;
            targetHotendTemperature = setTemp;
            _lastChangeTime = DateTime.Now; // Update the timestamp
            //_snackbar.Add($"Hotend temperature set to {setTemp}°C", Severity.Info);
        }

        public async Task ParseCurrentHotendTemperature(string input)
        {
            await Task.Run(() =>
            {
                Printer.ActivePrinter.HotendTemperatures.ParseCurrentTemperature(input);
                currentHotendTemperature = Printer.ActivePrinter.HotendTemperatures.CurrentTemp;
                if ((DateTime.Now - _lastChangeTime).TotalSeconds >= 3)
                {
                    targetHotendTemperature = Printer.ActivePrinter.HotendTemperatures.TargetTemp;
                }
            } );
        }

        public void ChangeFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildFilamentChangeCommand());
            //_snackbar.Add("Filament Change Command Sent", Severity.Info);
        }
        
        public void LoadFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildLoadFilamentCommand());
            //_snackbar.Add("Filament Load Command Sent", Severity.Info);
        }

        public void UnloadFilament()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildUnloadFilamentCommand());
            //_snackbar.Add("Filament Unload Command Sent", Severity.Info);
        }

        public void SetHotendPIDValues()
        {
            Printer.ActivePrinter.SerialConnection.Write(CommandMethods.BuildPIDAutoTuneCommand(0, PIDHotendTemp, PIDHotendCycles, true));
            //_snackbar.Add($"Setting PID Autotune for HOTEND {PIDHotendTemp}°C and {PIDHotendCycles} cycles!", Severity.Info);
        }
    }
}