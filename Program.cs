using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp
{
    internal class Program
    {
        //public static int CalculateChecksum(string command)
        //{
        //    byte[] bytes = command.Select(Convert.ToByte).ToArray();
        //    int checksum = bytes.Aggregate((x, y) => (byte)(x ^ y));
        //    return checksum;
        //}

        static void Main(string[] args)
        {

            SerialConnection.SerialInterface serialPort = new();

            serialPort.PortName = "COM4";
            serialPort.BaudRate = 1000000;

            Material.PreheatingProfiles preheat = new Material.PreheatingProfiles();

            preheat.MaterialIndex = 2;
            preheat.BedTemp = 60;
            preheat.HotendTemp = 230;
            preheat.FanSpeed = 0;

            Position position = new Position();
            position.YMovePosition = 50;
            position.XMovePosition = 50;
            position.ZMovePosition = 200;

            MotionSettingsData motionSettings = new MotionSettingsData();
            motionSettings.FeedRateFreeMove = 3500;


            serialPort.Open();

            string filePath = "C:/Users/mile mihailov/Desktop/bagholder_btt.gcode";
            string fileName = "box.GCO";
            Console.WriteLine(DateTime.Now);

            CommandMethods.SendInitSDCard();

            serialPort.TransferFileToSD(filePath, fileName, serialPort);

            serialPort.Write($"{CommandMethods.SendListSDCard()}");
            serialPort.Write(CommandMethods.SendSelectSDFile($"{fileName}"));
            serialPort.Write(CommandMethods.SendStartSDPrint());

            //serialPort.Write($"M30 {fileName}");

            serialPort.CheckForBusy();

            string input = serialPort.Read();
            Console.WriteLine(input);

            //serialPort.Close();

        }


    }
}