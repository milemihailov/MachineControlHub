using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Temps
{
    /// <summary>
    /// Represents temperature-related information for the printer's chamber.
    /// </summary>
    public class ChamberTemps : ITemperatures
    {
        private IPrinterConnection PrinterConnection { get; set; }


        public PIDValues PIDValues { get; set; }

        public ChamberTemps(IPrinterConnection printerConnection)
        {
            PrinterConnection = printerConnection;
        }

        public int CurrentTemp { get; set; }
        public int MaxTemp { get; set; }
        public int TargetTemp { get; set; }

        public void ParseCurrentTemperature(string input)
        {
            throw new System.NotImplementedException();
        }

        public void SetTemperature(int setTemp)
        {
            PrinterConnection.Write(CommandMethods.BuildSetChamberTempCommand(setTemp));
        }
    }

}
