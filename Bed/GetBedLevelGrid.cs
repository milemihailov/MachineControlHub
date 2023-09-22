namespace ControllingAndManagingApp.Bed
{
    public class GetBedLevelGrid
    {
        public const int LINES_TO_EXCLUDE = 4;

        public static string GetGrid(string input)
        {
            char[] delimiters = new char[] { '\r', '\n' };

            string[] splitedString = input.Split(delimiters);

            string grid = "";

            for (int i = 0; i < splitedString.Length - LINES_TO_EXCLUDE; i++)
            {
                grid += $"{splitedString[i]}\n";
            }
            return grid;
        }
    }
}
