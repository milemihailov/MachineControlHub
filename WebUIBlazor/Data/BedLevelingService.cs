using MachineControlHub.Bed;
using MachineControlHub.Motion;
using WebUI.Pages;

namespace WebUI.Data
{
    public class BedLevelingService
    {
        public static BedLevelData bedData;
        public string CSVData;

        public BedLevelingService() 
        {
            bedData = new BedLevelData();
        }

        public void CalibrateBed()
        {
            Data.ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildBedLevelingCommand());
            Data.ConnectionServiceSerial.printerConnection.CheckForBusy();
            string input = Data.ConnectionServiceSerial.printerConnection.Read();
            CSVData = bedData.GetGrid(input);
        }
    }
}
