using ControllingAndManagingApp.LogErrorHistory;
using ControllingAndManagingApp.Motion;
using ControllingAndManagingApp.SerialConnection;

namespace ControllingAndManagingApp.Print
{

    public class PrintService
    {
        const string GCODE_FILE_EXTENSION = ".gco";
        const string PRINT_ABORT_MESSAGE = "Print Aborted";

        private IPrinterConnection _serialInterface = new SerialInterface();


        /// <summary>
        /// Transfers a G-code file to the SD card using a serial connection.
        /// </summary>
        /// <param name="GcodeFilePath">The path to the G-code file to be transferred.</param>
        /// <param name="fileName">The name to assign to the file on the SD card.</param>
        /// <remarks>
        /// This method sends the G-code commands line by line to the printer's SD card via the serial connection.
        /// It starts by writing the file name and then sends the contents of the file, followed by a stop command.
        /// </remarks>
        /// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
        /// <exception cref="Exception">Thrown for any other errors that occur during the file transfer.</exception>
        public void TransferFileToSD(string GcodeFilePath, string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(GcodeFilePath))
                {
                    _serialInterface.Connect();
                    string line;
                    _serialInterface.Write($"{CommandMethods.BuildStartSDWriteCommand(fileName)}{GCODE_FILE_EXTENSION}");

                    while ((line = reader.ReadLine()) != null)
                    {
                        _serialInterface.Write(line);
                    }

                    _serialInterface.Write(CommandMethods.BuildStopSDWriteCommand());
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


        /// <summary>
        /// Initiates the printing process with the specified G-code file.
        /// </summary>
        /// <param name="gcodeFile">The path to the G-code file to be printed.</param>
        /// <remarks>
        /// This method prepares the printer to start printing by building and sending the necessary commands,
        /// including selecting the specified G-code file and starting the SD card print.
        /// </remarks>
        public void SelectAndStartPrint(string gcodeFile)
        {
            // Build and send the command to select the specified G-code file
            CommandMethods.BuildSelectSDFileCommand(gcodeFile);

            // Build and send the command to start the SD card print
            CommandMethods.BuildStartSDPrintCommand();
        }


        /// <summary>
        /// Aborts the current print by sending necessary commands to the provided serial interface.
        /// </summary>
        /// <param name="serial">The serial interface used for communication.</param>
        public void AbortCurrentPrint()
        {
            // Send commands to stop the SD print and set the LCD status.
            _serialInterface.Write(CommandMethods.BuildStopSDPrintCommand());
            _serialInterface.Write(CommandMethods.BuildSetLCDStatusCommand(PRINT_ABORT_MESSAGE));

            // Print a message to the console.
            Console.WriteLine(PRINT_ABORT_MESSAGE);
        }
    }
}