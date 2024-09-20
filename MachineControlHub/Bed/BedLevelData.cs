namespace MachineControlHub.Bed
{
    /// <summary>
    /// This class holds the data obtained from autobedleveling "G29"
    /// </summary>
    public class BedLevelData
    {
        public string BedLevelGridData { get; set; }
        public bool Processing { get; set; }

        public event EventHandler BedLevelingStateChanged;

        /// <summary>
        /// Get's the data from autobedlevel "G29" command
        /// </summary>
        /// <param name="input"></param>
        /// <returns>csv Grid</returns>
        public void GetGrid(string input)
        {
            var rows = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var csv = string.Join("\n", rows.Where(row => char.IsDigit(row.TrimStart().FirstOrDefault())) // Only include lines that start with a digit
                                           .Select(row => string.Join(",", row.Split(' ', StringSplitOptions.RemoveEmptyEntries))));
            int index = csv.IndexOf("X:");
            BedLevelGridData = index >= 0 ? csv.Substring(0, index) : csv;
        }

        private string CalibrateMessage { get; set; } = "";
        private string LoopMessage { get; set; } = "";
        private bool IsProcessingBilinear { get; set; }


        /// <summary>
        /// Handles updates from the printer.
        /// </summary>
        /// <param name="message">The message received from the printer.</param>
        public void OnBedLevelUpdate(string message)
        {
            LoopMessage += message;

            // get the bed leveling data
            if (LoopMessage.Contains("Bilinear"))
            {
                IsProcessingBilinear = true;
                CalibrateMessage += message;
                if (message.Contains("X:") || message.Contains("echo"))
                {
                    Processing = false;
                    IsProcessingBilinear = false;
                    GetGrid(CalibrateMessage);
                    CalibrateMessage = "";
                    BedLevelingStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            if (!IsProcessingBilinear)
            {
                LoopMessage = "";
            }
        }
    }
}