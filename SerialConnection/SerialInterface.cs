using ControllingAndManagingApp.LogErrorHistory;
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
        const int SLEEP_TIME_AFTER_BUSY_CHECK = 2000;

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
                    Console.WriteLine("Port opened");
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.LogError($"{ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Logger.LogError($"{ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"Unauthorized access when opening serial port: {ex.Message}");
            }
            catch (IOException ex)
            {
                Logger.LogError($"The port is in an invalid state. {ex.Message}");
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"The specified port on the current instance of the SerialPort is already open.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error opening serial port: {ex.Message}");
            }

        }


        /// <summary>
        /// Close the serial port connection.
        /// </summary>
        public void Close()
        {
            try
            {
                serialPort.Close();
                Console.WriteLine("Port closed");
            }
            catch (IOException ex)
            {
                Logger.LogError($"I/O error when closing serial port: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error closing serial port: {ex.Message}");
            }

        }


        /// <summary>
        /// Write data to the serial port.
        /// </summary>
        /// <param name="data">The data to be written to the serial port.</param>
        public void Write(string data)
        {
            serialPort.WriteTimeout = 1000;

            try
            {
                serialPort.WriteLine(data);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError($"The specified port is not open: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                Logger.LogError($"{ex.Message}");
            }
            catch (TimeoutException ex)
            {
                Logger.LogError($"Timeout when writing to serial port: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing to serial port: {ex.Message}");
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
                string data = serialPort.ReadExisting();
                return data;
            }
            catch (TimeoutException)
            {
                Logger.LogError("Timeout: No data received within the specified time.");
                return null;
            }
            catch (IOException ex)
            {
                Logger.LogError($"I/O error while reading from the serial port: {ex.Message}");
                return null;
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError($"Invalid operation while reading from the serial port: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Checks to see if printer is busy if this is the case sleeps for 1 second and outputs "busy" to the user then rechecks until it's no longer busy
        /// </summary>
        public void CheckForBusy()
        {
            try
            {
                string data = serialPort.ReadLine();

                while (data == BUSY_CHECK || !HasData())
                {
                    Console.WriteLine(serialPort.ReadLine());
                    Thread.Sleep(SLEEP_TIME_AFTER_BUSY_CHECK);
                }
            }
            catch (TimeoutException ex)
            {
                Logger.LogError($"Timeout while checking for busy: {ex.Message}");
            }
            catch (IOException ex)
            {
                Logger.LogError($"I/O error while checking for busy: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while checking for busy: {ex.Message}");
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

    }

}
