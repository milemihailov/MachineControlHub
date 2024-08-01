using MachineControlHub;
using MachineControlHub.Bed;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using System.Globalization;
using System.Text.Json;


namespace WebUI.Data
{

    public class BedLevelingService
    {

        public BedLevelData bedData;
        public PrinterManagerService printerManagerService;
        public BedLevelingService(PrinterManagerService printerManagerService)
        {
            bedData = new BedLevelData();
            this.printerManagerService = printerManagerService;
        }


        public void CalibrateBed(SerialConnection connection)
        {
            printerManagerService.ActivePrinter.BedLevelData.Processing = true;
            connection.Write(CommandMethods.BuildBedLevelingCommand());
        }
    }
}
