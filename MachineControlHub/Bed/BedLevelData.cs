namespace MachineControlHub.Bed
{
    /// <summary>
    /// This class holds the data obtained from autobedleveling "G29"
    /// </summary>
    public class BedLevelData
    {
        public string BedLevelGridData;


        /// <summary>
        /// Get's the data from autobedlevel "G29" command
        /// </summary>
        /// <param name="input"></param>
        /// <returns>csv Grid</returns>
        public string GetGrid(string input)
        {
            Console.WriteLine(input);
            var rows = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var csv = string.Join("\n", rows.Where(row => char.IsDigit(row.TrimStart().FirstOrDefault())) // Only include lines that start with a digit
                                           .Select(row => string.Join(",", row.Split(' ', StringSplitOptions.RemoveEmptyEntries))));
            int index = csv.IndexOf("X:");
            Console.WriteLine(index);
            return index >= 0 ? csv.Substring(0, index) : csv;
        }

    }
}