using System.Diagnostics;
using System.Text.RegularExpressions;
using MachineControlHub.LogErrorHistory;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Print
{

    public class PrintService
    {
        const string _gCODE_FILE_EXTENSION = ".gco";
        const string _pRINT_ABORT_MESSAGE = "Print Aborted";

        public double TransferToSDProgress { get; set; }
        public Stopwatch Stopwatch { get; set; } = new();
        public string TransferToSDTimeElapsed { get; set; }
        public string TransferToSDRemainingTime { get; set; }
        public bool TransferToSD { get; set; }

        public Action RefreshProgressState;


        private IPrinterConnection Connection { get; set; }
        public List<(string FileName, string FileContent, long FileSize)> UploadedFiles { get; set; } = new List<(string FileName, string FileContent, long FileSize)>();

        public PrintService(IPrinterConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Calculate checksum of a line from given input
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static int CalculateChecksum(string command)
        {
            int checksum = 0;
            foreach (char c in command)
            {
                unchecked
                {
                    checksum ^= c; // XOR each character
                }
            }
            return checksum;
        }

        /// <summary>
        /// Removes unwanted characters from given string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charactersToRemove"></param>
        /// <returns>Empty string</returns>
        static string RemoveCharacters(string input, string charactersToRemove)
        {
            foreach (char c in charactersToRemove)
            {
                input = input.Replace(c.ToString(), string.Empty);
            }
            return input;
        }

        /// <summary>
        /// Extracts the part of the line up to the first occurrence of the semicolon.
        /// </summary>
        /// <param name="line">The input line containing the G-code command.</param>
        /// <returns>The part of the line before the first semicolon, or the entire line if no semicolon is found.</returns>
        static string ExtractLineBeforeComment(string line)
        {
            int semicolonIndex = line.IndexOf(';');
            if (semicolonIndex >= 0)
            {
                return line.Substring(0, semicolonIndex).Trim();
            }
            return line.Trim();
        }

        /// <summary>
        /// Transfers a G-code file to the SD card using a serial connection.
        /// </summary>
        /// <param name="GcodeFile">The path to the G-code file to be transferred.</param>
        /// <param name="fileName">The name to assign to the file on the SD card.</param>
        /// <remarks>
        /// This method sends the G-code commands line by line to the printer's SD card via the serial connection.
        /// It starts by writing the file name and then sends the contents of the file, followed by a stop command.
        /// </remarks>
        /// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
        /// <exception cref="Exception">Thrown for any other errors that occur during the file transfer.</exception>
        public async Task TransferFileToSD(string GcodeContent, string fileName)
        {
            await Task.Run(async () =>
            {
                try
                {
                    TransferToSD = true;
                    Stopwatch.Reset();
                    Stopwatch.Start();

                    // Adds all the content in a list (seemed easier to filter from unwanted characters from the firmware)
                    List<string> lines = new();
                    using (StringReader reader = new(GcodeContent))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.StartsWith(";"))
                            {
                                lines.Add("");
                            }
                            else
                            {
                                string cleanedLine = ExtractLineBeforeComment(line);
                                lines.Add(cleanedLine);
                            }
                        }
                    }

                    // Remove all empty lines after adding
                    lines.RemoveAll(l => string.IsNullOrWhiteSpace(l));

                    // Set line number and initialize the SD write process
                    Connection.Write("M110 N0");
                    Connection.Write($"{CommandMethods.BuildStartSDWriteCommand(fileName)}{_gCODE_FILE_EXTENSION}");

                    int retryCount = 0;
                    bool success = true;

                    // Sends all lines to the SD attached to printer
                    for (int i = 1; i < lines.Count; i++)
                    {
                        if (lines[i - 1] != string.Empty)
                        {
                            // Removes unnessecary characters and then calculates checksum
                            string cleanedLine = RemoveCharacters(lines[i - 1], ";,");
                            int checkSum = CalculateChecksum($"N{i} {cleanedLine}");

                            // Formated line to send to the SD
                            string lineToSend = $"N{i} {cleanedLine}*{checkSum}\n";

                            // Send the line
                            Connection.Write(lineToSend);

                            // If error when sending a line it will retry to send the error line
                            string input = await Connection.ReadAllAsync();
                            if (input.Contains("Resend"))
                            {
                                Console.WriteLine(input);
                                Match resendMatch = Regex.Match(input, @"(?<=Resend:\s*)\d+");
                                if (resendMatch.Success)
                                {
                                    int resendLine = int.Parse(resendMatch.Value);
                                    i = resendLine;
                                    checkSum = CalculateChecksum($"N{i} {cleanedLine}");
                                    lineToSend = $"N{resendLine} {cleanedLine}*{checkSum}\n";
                                    success = false;
                                }
                                else
                                {
                                    throw new FormatException("Unexpected Resend format: " + input);
                                }
                            }

                            // Event to update the progress in the UI
                            RefreshProgressState?.Invoke();

                            // Progress in percentage
                            TransferToSDProgress = Math.Round(((double)i / (lines.Count - 1)) * 100, 1);

                            // Time elapsed
                            TimeSpan TimeElapsed = TimeSpan.FromMilliseconds(Stopwatch.ElapsedMilliseconds);
                            TransferToSDTimeElapsed = string.Format($"{TimeElapsed.Hours:D2}:{TimeElapsed.Minutes:D2}:{TimeElapsed.Seconds:D2}");

                            //// Estimate remaining time
                            //double progressPercentage = (double)i / lines.Count;
                            //double timePerLine = Stopwatch.ElapsedMilliseconds / (double)i; // Time spent per line
                            //long remainingTimeMs = (long)(timePerLine * (lines.Count - i)); // Estimate remaining time in milliseconds
                            //TimeSpan remainingTime = TimeSpan.FromMilliseconds(remainingTimeMs);

                            EstimateRemainingTime(lines.Count, i);

                            //// Format the remaining time
                            //TransferToSDRemainingTime = string.Format($"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}");

                            // Retry up to 3 times
                            while (!success && retryCount < 3)
                            {
                                Connection.Write(lineToSend);
                                string response = await Connection.ReadAllAsync();
                                Console.WriteLine(lineToSend);

                                retryCount++;
                            }
                            success = true;
                            retryCount = 0;
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Logger.LogError($"File not found. {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"An error occurred: {ex.Message}");
                }

                Connection.Write(CommandMethods.BuildStopSDWriteCommand());
                Stopwatch.Stop();
                TransferToSD = false;
            });
        }

        private void EstimateRemainingTime(int linesCount, int iterator)
        {
            // Estimate remaining time
            //double progressPercentage = (double)iterator / linesCount;
            double timePerLine = Stopwatch.ElapsedMilliseconds / (double)iterator; // Time spent per line
            long remainingTimeMs = (long)(timePerLine * (linesCount - iterator)); // Estimate remaining time in milliseconds
            TimeSpan remainingTime = TimeSpan.FromMilliseconds(remainingTimeMs);

            // Format the remaining time
            TransferToSDRemainingTime = string.Format($"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}");
        }


        public void StartPrint(string gcodeFile)
        {
            // Build and send the command to select the specified G-code file
            Connection.Write(CommandMethods.BuildSelectSDFileCommand(gcodeFile));
            // Build and send the command to start the SD card print
            Connection.Write(CommandMethods.BuildStartSDPrintCommand());
        }


        /// <summary>
        /// Aborts the current print by sending necessary commands to the provided serial interface.
        /// </summary>
        /// <param name="serial">The serial interface used for communication.</param>
        public void AbortCurrentPrint()
        {
            // Send commands to stop the SD print and set the LCD status.
            Connection.Write(CommandMethods.BuildStopSDPrintCommand());
            Connection.Write(CommandMethods.BuildSetLCDStatusCommand(_pRINT_ABORT_MESSAGE));

            // Print a message to the console.
            Console.WriteLine(_pRINT_ABORT_MESSAGE);
        }

        /// <summary>
        /// Reads and lists all the files from the media attached to the printer
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns>List of gcode files</returns>
        public List<(string FileName, string FileSize)> ListSDFiles(string inputText)
        {
            string pattern = @"Begin file list([\s\S]+?)End file list";

            Match match = Regex.Match(inputText, pattern, RegexOptions.None);

            string parsedString = "";

            if (match.Success)
            {
                parsedString = match.Groups[1].Value.Trim();
            }

            List<(string FileName, string FileSize)> files = new List<(string FileName, string FileSize)>();

            string[] extractedSdFiles = parsedString.Split('\n');

            foreach (string sdFile in extractedSdFiles)
            {
                string[] parts = sdFile.Split(' ');
                if (parts.Length >= 2)
                {
                    string fileName = parts[0];
                    string fileSize = parts[1];
                    files.Add((fileName, fileSize));
                }
            }
            return files;
        }

        /// <summary>
        /// Reads and lists all the files with long names from the media attached to the printer
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns>List of gcode files</returns>
        public List<(string FileName, string FileSize)> ListLongNameSDFiles(string input)
        {
            string pattern = @"(\d+)\s0x[A-Fa-f0-9]+\s(.+?)\r?\n";
            MatchCollection matches = Regex.Matches(input, pattern);

            List<(string FileName, string FileSize)> files = new List<(string FileName, string FileSize)>();

            foreach (Match match in matches)
            {
                files.Add((match.Groups[2].Value, match.Groups[1].Value));
            }
            return files;
        }
    }


}