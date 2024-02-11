using MachineControlHub.Bed;
using MachineControlHub.Motion;
using Plotly.Blazor;
using WebUI.Pages;

namespace WebUI.Data
{
    public class BedLevelingService
    {
        public static BedLevelData bedData;
        public string CSVData;
        public Task<IList<ITrace>> meshData;

        public BedLevelingService() 
        {
            bedData = new BedLevelData();
        }

        public void CalibrateBed()
        {
            ConnectionServiceSerial.printerConnection.Write(CommandMethods.BuildBedLevelingCommand());
            ConnectionServiceSerial.printerConnection.CheckForBusy();
            string input = ConnectionServiceSerial.printerConnection.Read();
            CSVData = bedData.GetGrid(input);
        }
    }
}
