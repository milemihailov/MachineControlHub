namespace MachineControlHub.LogErrorHistory
{
    public static class Logger
    {
        /// <summary>
        /// The path to the log file.
        /// </summary>
        public static string logPath = @"ErrorHistory.txt";

        /// <summary>
        /// The current working directory of the application.
        /// </summary>
        public static string workingDirectory = Environment.CurrentDirectory;

        /// <summary>
        /// The full path to the log file, including the working directory.
        /// </summary>
        public static string fullPath = Path.Combine(workingDirectory, logPath);


        /// <summary>
        /// Logs an error message to the console and a text file.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void LogError(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(fullPath, $"{DateTime.Now} - {message}\n");
        }
    }
}
