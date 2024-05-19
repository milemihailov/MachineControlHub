using MachineControlHub.LogErrorHistory;
using MachineControlHub.Motion;
using MachineControlHub.PrinterConnection;
using System.Text.RegularExpressions;

namespace MachineControlHub.Print
{

    public class PrintService
    {
        const string GCODE_FILE_EXTENSION = ".gco";
        const string PRINT_ABORT_MESSAGE = "Print Aborted";

        private IPrinterConnection _connection;

        public PrintService(IPrinterConnection connection)
        {
            _connection = connection;
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
                    _connection.Write($"{CommandMethods.BuildStartSDWriteCommand(fileName)}{GCODE_FILE_EXTENSION}");

                    while ((line = reader.ReadLine()) != null)
                    {
                        _connection.Write(line);
                        Console.WriteLine(line);
                    }

                    _connection.Write(CommandMethods.BuildStopSDWriteCommand());
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
            _connection.Write(CommandMethods.BuildSelectSDFileCommand(gcodeFile));
            // Build and send the command to start the SD card print
            _connection.Write(CommandMethods.BuildStartSDPrintCommand());
        }


        /// <summary>
        /// Aborts the current print by sending necessary commands to the provided serial interface.
        /// </summary>
        /// <param name="serial">The serial interface used for communication.</param>
        public void AbortCurrentPrint()
        {
            // Send commands to stop the SD print and set the LCD status.
            _connection.Write(CommandMethods.BuildStopSDPrintCommand());
            _connection.Write(CommandMethods.BuildSetLCDStatusCommand(PRINT_ABORT_MESSAGE));

            // Print a message to the console.
            Console.WriteLine(PRINT_ABORT_MESSAGE);
        }

        


        public List<string> ListSDFiles(string inputText)
        {

            string pattern = @"Begin file list([\s\S]+?)End file list";

            //_connection.Write(CommandMethods.BuildListSDCardCommand());
            //Thread.Sleep(200);
            //string inputText = _connection.ReadAll();
            Console.WriteLine(inputText);
            Match match = Regex.Match(inputText, pattern, RegexOptions.None);

            string parsedString = "";

            if (match.Success)
            {
                parsedString = match.Groups[1].Value.Trim();
            }

            List<string> files = new List<string>();

            string[] extractedSdFiles = parsedString.Split('\n');

            foreach (string sdFile in extractedSdFiles)
            {
                files.Add(sdFile);
            }
            return files;
        }
    }
}