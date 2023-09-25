namespace ControllingAndManagingApp.Bed
{

    public class BedLevelData
    {
        public const int LINES_TO_EXCLUDE = 4;

        public string BedLevelGridData;


        /// <summary>
        /// Get's the data from autobedlevel "G29" command
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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