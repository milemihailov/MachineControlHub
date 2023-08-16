using ControllingAndManagingApp.Gcode;
using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GCodeCommandsData command = new GCodeCommandsData();
            Position position = new Position();
            MotionSettingsData motionSettingsData = new MotionSettingsData();

            position.XMovePosition = 1;
            position.YMovePosition = 2;
            position.ZMovePosition = 0;
            position.XHomePosition = 0;
            position.YHomePosition = 3;
            position.ZHomePosition = 2;
            motionSettingsData.FeedRate = 1;
            Print.Print filename = new Print.Print();
            filename.File = "fee";

            Console.WriteLine(CommandMethods.HomeAxes(position));

            Console.WriteLine(CommandMethods.StartSDWrite(filename.File));
        }

    }
}