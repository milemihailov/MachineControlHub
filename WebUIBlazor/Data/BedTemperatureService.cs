using MachineControlHub.Temps;

namespace WebUI.Data
{
    public class BedTemperatureService
    {
        public BedTemps bed;
        public BedTemperatureService() 
        {
            bed = new BedTemps(Data.ConnectionServiceSerial.printerConnection);
        }

        public void SetBedTemperature() 
        {
            bed.SetBedTemperature();
        }

        public void ParseCurrentBedTemperature()
        {
            bed.ParseCurrentTemperature();
        }
    }
}
