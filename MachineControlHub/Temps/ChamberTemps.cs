using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Temps
{
    /// <summary>
    /// Represents temperature-related information for the printer's chamber.
    /// </summary>
    public class ChamberTemps : ITemperatures
    {
        private IPrinterConnection _printerConnection;


        public PIDValues PIDValues { get; set; }

        public ChamberTemps(IPrinterConnection printerConnection)
        {
            _printerConnection = printerConnection;
        }

        public int CurrentTemp { get; set; }
        public int MaxTemp { get; set; }
        public int SetTemp { get; set; }
        public int TargetTemp { get; set; }

        public void ParseCurrentTemperature(string input)
        {
            throw new System.NotImplementedException();
        }

        public void SetTemperature(int setTemp)
        {
            _printerConnection.Write(CommandMethods.BuildSetChamberTempCommand(setTemp));
        }
    }

}
