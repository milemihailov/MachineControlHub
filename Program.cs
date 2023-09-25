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



            serialPort.Open();
            serialPort.Write(CommandMethods.SendFanOff());
            serialPort.CheckForBusy();

            string input = serialPort.Read();

            Console.WriteLine(input);

            serialPort.Close();
        }

    }
}