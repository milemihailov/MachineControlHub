using MachineControlHub;

namespace WebUI.Data
{
    public class ChamberTemperatureService
    {


        public int CurrentChamberTemp { get; set; }
        public int TargetChamberTemp { get; set; }
        public int PIDChamberCycles { get; set; }
        public int PIDChamberTemp { get; set; }


        public void SetChamberTemperature(int setTemp, Printer printer)
        {
            printer.ChamberTemperatures.SetTemperature(setTemp);
        }

        public async Task ParseCurrentChamberTemperature(string input, Printer printer)
        {
            await Task.Run(() =>
            {
                printer.ChamberTemperatures.ParseCurrentTemperature(input);
                CurrentChamberTemp = printer.ChamberTemperatures.CurrentTemp;
                TargetChamberTemp = printer.ChamberTemperatures.TargetTemp;
            });
        }

    }
}
