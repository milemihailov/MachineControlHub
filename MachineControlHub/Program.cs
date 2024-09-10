using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub
{
    internal class Program
    {

        static void Main(string[] args)
        {

            string command = "N17 M300 P30";

            int CalculateChecksum(string command)
            {
                int checksum = 0;
                foreach (char c in command)
                {
                    checksum ^= c; // XOR each character's ASCII value
                }
                return checksum;
            }

            int checksum = CalculateChecksum(command);
            Console.WriteLine(checksum);
        }


    }
}