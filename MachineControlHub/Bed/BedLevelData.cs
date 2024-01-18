namespace MachineControlHub.Bed
{
    /// <summary>
    /// This class holds the data obtained from autobedleveling "G29"
    /// </summary>
    public class BedLevelData
    {
        public const int LINES_TO_EXCLUDE = 2;

        public string BedLevelGridData;


        /// <summary>
        /// Get's the data from autobedlevel "G29" command
        /// </summary>
        /// <param name="input"></param>
        /// <returns>csv Grid</returns>
        public string GetGrid(string input)
        {
            char[] delimiters = new char[] { '\r', '\n' };

            string[] rows = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            string csv = "";

            for (int i = 0; i < rows.Length - LINES_TO_EXCLUDE; i++)
            {
                string[] columns = rows[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < columns.Length; j++)
                {
                    csv += columns[j];

                    if (j < columns.Length - 1)
                    {
                        csv += ",";
                    }
                }
                csv += "\n";
            }
            return csv;
        }
    }
}