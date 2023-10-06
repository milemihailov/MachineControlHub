using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            SerialConnection.SerialConnection serialPort = new();

            serialPort.PortName = "COM4";
            serialPort.BaudRate = 115200;

            Material.PreheatingProfiles preheat = new Material.PreheatingProfiles();

            preheat.MaterialIndex = 2;
            preheat.BedTemp = 60;
            preheat.HotendTemp = 230;
            preheat.FanSpeed = 0;

            Position position = new Position();
            position.YMovePosition = 40;

            MotionSettingsData motionSettings = new MotionSettingsData();

            Print.Print na = new Print.Print();
            na.ParseExtractedSettingsFromPrintedFile("C:/Users/mile mihailov/Desktop/V29 Whistle.gcode");

            //string result = na.StartTimeOfPrint;


            //string a = na.StartTimeOfPrint.ToString();
            Console.WriteLine(na.ExtractedSettingsFromPrintedFile);
            //serialPort.Open();
            //Console.WriteLine(serialPort.Read());

            //serialPort.CheckForBusy();

            //string input = serialPort.Read();
            //Console.WriteLine(input);

            //serialPort.Close();
        }

    }
}