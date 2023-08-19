using ControllingAndManagingApp.Gcode;
using ControllingAndManagingApp.Material;
using ControllingAndManagingApp.Motion;

namespace ControllingAndManagingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GCodeCommands command = new GCodeCommands();
            Position position = new Position();
            MotionSettingsData motionSettingsData = new MotionSettingsData();

            position.XMovePosition = 1;
            position.YMovePosition = 2;
            position.ZMovePosition = 0;
            position.XHomePosition = 0;
            position.YHomePosition = 3;
            position.ZHomePosition = 2;
            motionSettingsData.FeedRateFreeMove = 1;
            Print.Print filename = new Print.Print();
            filename.File = "fee";

            Temps.BedTemps hotendTemps = new Temps.BedTemps();
            hotendTemps.BedSetTemp = 220;

            //motionSettingsData.FanSpeed = 200;

            PreheatingProfiles profiles = new PreheatingProfiles();
            profiles.HotendTemp = 200;
            profiles.BedTemp = 300;
            //profiles.FanSpeed = 32;
            profiles.MaterialIndex = 2;
            FilamentProperties fil = new FilamentProperties();
            fil.FilamentDiameter = 1;

            motionSettingsData.EMaxFeedrate = 1;
            motionSettingsData.ZHomeOffset = 2;

            Console.WriteLine(CommandMethods.HomeAxes(position));

            Console.WriteLine(CommandMethods.DeleteSDFile(filename.File));
        }

    }
}