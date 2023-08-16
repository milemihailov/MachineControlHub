namespace ControllingAndManagingApp.Gcode
{
    public class GCodeCommandsData
    {
        public char Type;
        public int Instruction;
        public List<string> Parameters;


        public string ParametersString()
        {
            if (Parameters != null)
            {
                return String.Join(" ", Parameters);
            }
            return null;
        }
    }
}