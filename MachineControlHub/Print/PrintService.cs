using MachineControlHub.LogErrorHistory;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using System.Text.RegularExpressions;

namespace MachineControlHub.Print
{

    public class PrintService
    {
        const string _gCODE_FILE_EXTENSION = ".gco";
        const string _pRINT_ABORT_MESSAGE = "Print Aborted";

        private IPrinterConnection Connection { get; set;}
        public List<(string FileName, string FileContent, long FileSize)> UploadedFiles { get; set; } = new List<(string FileName, string FileContent, long FileSize)>();

        public PrintService(IPrinterConnection connection)
        {
            Connection = connection;
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
        public void TransferFileToSD(string GcodeContent, string fileName)
        {
            try
            {
                using (StringReader reader = new StringReader(GcodeContent))
                {
                    string line;
                    Connection.Write($"{CommandMethods.BuildStartSDWriteCommand(fileName)}{_gCODE_FILE_EXTENSION}");

                    while ((line = reader.ReadLine()) != null)
                    {
                        Connection.Write(line);
                        Console.WriteLine(line);
                    }

                    Connection.Write(CommandMethods.BuildStopSDWriteCommand());
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

        public List<(string FileName, string FileSize)> ListLongNameSDFiles(string input)
        {
            string pattern = @"(\d+)\s0x[A-Fa-f0-9]+\s(.+?)\r?\n";
            MatchCollection matches = Regex.Matches(input, pattern);

            List<(string FileName, string FileSize)> files = new List<(string FileName, string FileSize)>();

            foreach (Match match in matches)
            {
                files.Add((match.Groups[2].Value, match.Groups[1].Value));
            }
            Console.WriteLine(files);
            return files;
        }
    }

    
}