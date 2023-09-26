namespace ControllingAndManagingApp.Gcode
{

    /// <summary>
    /// Provides methods for generating G-code command strings.
    /// </summary>
    public class GCodeMethods
    {

        /// <summary>
        /// Generates a G-code command string with the specified G-code command, text parameter, and semicolon.
        /// </summary>
        /// <param name="gcode">The G-code command.</param>
        /// <param name="text">The text parameter.</param>
        /// <returns>The generated G-code command string.</returns>
        public static string GCodeString(GCodeCommands gcode, string text)
        {
            return $"{gcode.Type}{gcode.Instruction} {text};";
        }

        /// <summary>
        /// Generates a G-code command string with the specified G-code command, integer value, and "S" prefix.
        /// </summary>
        /// <param name="gcode">The G-code command.</param>
        /// <param name="value">The integer value.</param>
        /// <returns>The generated G-code command string.</returns>
        public static string GCodeString(GCodeCommands gcode, int value)
        {
            return $"{gcode.Type}{gcode.Instruction} S{value};";
        }

        /// <summary>
        /// Generates a G-code command string with the specified G-code command and its associated parameters.
        /// </summary>
        /// <param name="gcode">The G-code command.</param>
        /// <returns>The generated G-code command string.</returns>
        public static string GCodeString(GCodeCommands gcode)
        {
            return $"{gcode.Type}{gcode.Instruction} {gcode.ParametersString()};".ToUpper();
        }
    }
}
