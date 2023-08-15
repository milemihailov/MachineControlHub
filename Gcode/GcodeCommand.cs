namespace ControllingAndManagingApp.Gcode
{
    public class GcodeCommand
    {
        public char Type;
        public int Instruction;
        public List<string> Parameters;


        public string GcodeLine()
        {
            return $"{Type}{Instruction} {GetParameters()};".ToUpper();
        }


        public string GetParameters()
        {
            if (Parameters != null)
            {
                return String.Join(" ", Parameters);
            }
            return null;
        }
    }
}