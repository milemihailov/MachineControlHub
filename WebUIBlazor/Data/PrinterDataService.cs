using MachineControlHub;
using MachineControlHub.PrinterConnection;
using MachineControlHub.Temps;

namespace WebUI.Data
{
    public class PrinterData
    {
        public readonly ConnectionServiceSerial serialConnection;
        //public PrinterData Printer { get; set; }
        public bool IsConnected { get; set; }

        public Printer Printer = new();

        public PrinterData()
        {
            Printer.PrinterConnection = (IPrinterConnection)serialConnection;
            //init connection
            //serial inint
            //hotend inint
            Printer.HotendTemperatures = new HotendTemps(Printer.PrinterConnection);
            //////update hotend 
            ////HotendTemperatures.ParseCurrentTemperature();

            Printer.BedTemperatures = new BedTemps(Printer.PrinterConnection);
            //Printer.BedTemperatures.ParseCurrentTemperature();

        }
    }
}