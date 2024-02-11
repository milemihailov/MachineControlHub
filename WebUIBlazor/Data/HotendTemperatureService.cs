using MachineControlHub.Motion;
using MachineControlHub.Temps;

namespace WebUI.Data
{
    public class HotendTemperatureService
    {
        public HotendTemps hotend;
        public int currentHotendTemperature;
        public int setHotendTemperature;
        public int targetHotendTemperature;

        public HotendTemperatureService()
        {
            hotend = new HotendTemps(ConnectionServiceSerial.printerConnection);
            
        }
        public void SetHotendTemperature(int setTemp)
        {
            hotend.SetHotendTemperature(setTemp);
        }
        public void ParseCurrentHotendTemperature()
        {
            hotend.ParseCurrentHotendTemperature();
            currentHotendTemperature = hotend.HotendCurrentTemp;
            setHotendTemperature = hotend.SetHotendTemp;
            targetHotendTemperature = hotend.TargetHotendTemp;
        }

        public void ChangeFilament()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildFilamentChangeCommand());
        }
        
        public void LoadFilament()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildLoadFilamentCommand());
        }

        public void UnloadFilament()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildUnloadFilamentCommand());
        }
    }
}
