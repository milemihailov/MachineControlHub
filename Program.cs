using ControllingAndManagingApp.Gcode;
using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GcodeCommand command = new GcodeCommand();
            Position position = new Position();
            MotionSettingsData motionSettingsData = new MotionSettingsData();

            position.XMovePosition = 1;
            position.YMovePosition = 2;
            position.ZMovePosition = 0;
            position.XHomePosition = 0;
            position.YHomePosition = 3;
            position.ZHomePosition = 2;
            motionSettingsData.FeedRate = 1;

            Console.WriteLine(Commands.HomeAxes(position));
            //Console.WriteLine(Commands.DisableSteppers());
            Console.WriteLine(Commands.RapidLinearMove(position, motionSettingsData));
        }

    }
}