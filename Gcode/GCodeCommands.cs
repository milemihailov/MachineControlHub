namespace ControllingAndManagingApp.Gcode
{
    /// <summary>
    /// Represents a G-code command with its type, instruction, and parameters.
    /// </summary>
    public class GCodeCommands
    {
        /// <summary>
        /// Gets or sets the type of the G-code command (e.g., 'G', 'M').
        /// </summary>
        public char Type { get; set; }

        /// <summary>
        /// Gets or sets the instruction code of the G-code command (e.g., 1, 28).
        /// </summary>
        public int Instruction { get; set; }

        /// <summary>
        /// Gets or sets the list of parameters associated with the G-code command.
        /// </summary>
        public List<string> Parameters { get; set; }

        /// <summary>
        /// Converts the list of parameters into a space-separated string.
        /// </summary>
        /// <returns>A string representation of the parameters or null if no parameters are available.</returns>
        public string ParametersString()
        {
            if (Parameters != null)
            {
                return string.Join(" ", Parameters);
            }
            return null;
        }
    }

}