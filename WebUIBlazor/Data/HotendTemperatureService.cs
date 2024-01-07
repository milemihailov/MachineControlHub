using MachineControlHub.Temps;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public HotendTemps hotend;

        public HotendTemperatureService()
        {
            hotend = new HotendTemps(Data.ConnectionServiceSerial.printerConnection);
        }
        public void SetHotendTemperature(int setTemp)
        {
            hotend.SetHotendTemperature(setTemp);
        }
        public void ParseCurrentHotendTemperature()
        {
            hotend.ParseCurrentHotendTemperature();
        }
    }
}
