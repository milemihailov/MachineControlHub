using MachineControlHub.Temps;

namespace WebUI.Data
{
    public class BedTemperatureService
    {
        public BedTemps bed;
        public int currentBedTemperature;
        public int setBedTemperature;
        public int targetBedTemperature;

        public BedTemperatureService() 
        {
            bed = new BedTemps(Data.ConnectionServiceSerial.printerConnection);
        }

        public void SetBedTemperature(int setTemp) 
        {
            bed.SetBedTemperature(setTemp);
        }

        public void ParseCurrentBedTemperature()
        {
            bed.ParseCurrentTemperature();
            currentBedTemperature = bed.CurrentBedTemp;
            setBedTemperature = bed.SetBedTemp;
            targetBedTemperature = bed.TargetBedTemp;
        }
    }
}
