using MachineControlHub.Temps;
using MudBlazor;

namespace WebUI.Data
{
    public class ChamberTemperatureService
    {
        public PrinterManagerService Printer { get; set; }


        public int currentChamberTemperature;
        public int setChamberTemperature;
        public int targetChamberTemperature;
        public int PIDChamberCycles;
        public int PIDChamberTemp;

        public ChamberTemperatureService(PrinterManagerService printer)
        {
            Printer = printer;
        }

        public void SetChamberTemperature(int setTemp)
        {
            Printer.ActivePrinter.ChamberTemperatures.SetTemperature(setTemp);
        }

        public void ParseCurrentChamberTemperature(string input)
        {
            Printer.ActivePrinter.ChamberTemperatures.ParseCurrentTemperature(input);
            currentChamberTemperature = Printer.ActivePrinter.ChamberTemperatures.CurrentTemp;
            setChamberTemperature = Printer.ActivePrinter.ChamberTemperatures.SetTemp;
            targetChamberTemperature = Printer.ActivePrinter.ChamberTemperatures.TargetTemp;
        }

    }
}
