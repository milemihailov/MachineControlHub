namespace ControllingAndManagingApp.Gcode
{
    public class GCodeMethods
    {
        /// <summary>
        ///  Takes data from GCodeCommandsData.cs and Print.cs and returns a string.
        /// </summary>
        /// <param name="gcode"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GCodeString(GCodeCommands gcode, string text)
        {
            return $"{gcode.Type}{gcode.Instruction} {text};";
        }

        public static string GCodeString(GCodeCommands gcode, int value)
        {
            return $"{gcode.Type}{gcode.Instruction} S{value};";
        }

        public static string GCodeString(GCodeCommands gcode)
        {
            return $"{gcode.Type}{gcode.Instruction} {gcode.ParametersString()};".ToUpper();
        }

    }
}
