using ControllingAndManagingApp.Motion;
using System.IO.Ports;

namespace ControllingAndManagingApp.SerialConnection
{
    /// <summary>
    /// This class provides methods to open, close, read, and write data to a serial port.
    /// It also includes features for configuring serial port settings such as port name, baud rate,
    /// data bits, stop bits, rts enable, dtr enable and parity.
    /// </summary>
    public class SerialInterface
    {
        const string BUSY_CHECK = "echo:busy: processing\n";
        const string GCODE_FILE_EXTENSION = ".gco";
        private SerialPort serialPort;

        public SerialInterface()
        {
            serialPort = new SerialPort();
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = true;

        }

        public string PortName
        {
            get { return serialPort.PortName; }
            set { serialPort.PortName = value; }
        }

        public int BaudRate
        {
            get { return serialPort.BaudRate; }
            set { serialPort.BaudRate = value; }
        }


        /// <summary>
        /// Open the serial port connection.
        /// </summary>
        public void Open()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    Thread.Sleep(500);
                    Console.WriteLine("Port opened");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                Console.WriteLine($"Error opening serial port: {ex.Message}");
            }

        }


        /// <summary>
        /// Close the serial port connection.
        /// </summary>
        public void Close()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    Console.WriteLine("Port closed");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                Console.WriteLine($"Error closing serial port: {ex.Message}");
            }

        }


        /// <summary>
        /// Write data to the serial port.
        /// </summary>
        /// <param name="data">Write string data</param>
        public void Write(string data)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                Console.WriteLine($"Error writing to serial port: {ex.Message}");
            }
        }


        /// <summary>
        /// Read data from the serial port.
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            serialPort.ReadTimeout = 5000;

            try
            {
                if (serialPort.IsOpen)
                {
                    string data = serialPort.ReadExisting();
                    return data;
                }
                return null;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout: No data received within the specified time.");
                return null;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                Console.WriteLine($"Error reading from serial port: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Checks to see if printer is busy if this is the case sleeps for 1 second and outputs "busy" to the user then rechecks until it's no longer busy
        /// </summary>
        public void CheckForBusy()
        {
            string data = serialPort.ReadLine();

            while (data == BUSY_CHECK || !HasData())
            {
                Console.WriteLine(serialPort.ReadLine());
                Thread.Sleep(2000);
            }
        }


        /// <summary>
        /// Checks on opened port to see if there is data left to read
        /// </summary>
        /// <returns></returns>
        public bool HasData()
        {
            if (serialPort.IsOpen && serialPort.BytesToRead > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Makes a list of available ports
        /// </summary>
        /// <returns></returns>
        public string AvailablePortNames()
        {
            List<string> availablePorts = SerialPort.GetPortNames().ToList();
            string ports = "";

            foreach (string port in availablePorts)
            {
                ports += port;
            }
            return ports;
        }



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
        public void TransferFileToSD(string GcodeFilePath, string fileName, SerialInterface serial)
        {
            try
            {
                using (StreamReader reader = new StreamReader(GcodeFilePath))
                {
                    string line;
                    serial.Write($"{CommandMethods.SendStartSDWrite(fileName)}{GCODE_FILE_EXTENSION}");

                    while ((line = reader.ReadLine()) != null)
                    {
                        serial.Write(line);
                        Console.WriteLine(line);
                    }

                    serial.Write(CommandMethods.SendStopSDWrite());
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
    }

}
