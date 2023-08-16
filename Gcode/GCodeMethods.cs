namespace ControllingAndManagingApp.Gcode
{
    public class GCodeMethods
    {
        /// <summary>
        ///  Takes data from GCodeCommandsData.cs and Print.cs and returns a string.
        /// </summary>
        /// <param name="gcode"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GCodeString(GCodeCommandsData gcode, string filename)
        {
            return $"{gcode.Type}{gcode.Instruction} {gcode.ParametersString()} {filename};";
        }

        public static string GCodeString(GCodeCommandsData gcode)
        {
            return $"{gcode.Type}{gcode.Instruction} {gcode.ParametersString()};".ToUpper();
        }

    }
}
