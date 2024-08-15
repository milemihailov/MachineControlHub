using MachineControlHub.LogErrorHistory;
using System.IO.Ports;

namespace MachineControlHub.PrinterConnection
{
    /// <summary>
    /// This class provides methods to open, close, read, and write data to a serial port.
    /// It also includes features for configuring serial port settings such as port name, baud rate,
    /// data bits, stop bits, rts enable, dtr enable and parity.
    /// </summary>
    public class SerialConnection : IPrinterConnection
    {
        const string _bUSY_CHECK = "echo:busy: processing\n";
        const int _sLEEP_TIME_AFTER_BUSY_CHECK = 1500;

        public event Action Disconnected;

        private SerialPort _serialPort;
        public bool IsConnected { get; set; }
        public SerialConnection()
        {
            _serialPort = new SerialPort
            {
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.None,
                RtsEnable = true,
                DtrEnable = true
            };
        }

        public void Initialize(string connectionString)
        {
            string[] connectionParams = connectionString.Split(',');
            string portName = connectionParams[0].Trim();
            int baudRate = int.Parse(connectionParams[1].Trim());

            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
        }


        /// <summary>
        /// Gets or sets the name of the serial port.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when access to the serial port is not authorized.</exception>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is provided.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="Exception">Thrown for other general errors.</exception>
        public string PortName
        {
            get { return _serialPort.PortName; }
            set
            {
                try
                {
                    _serialPort.PortName = value;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"An error occurred while setting the PortName: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Baud Rate for the serial port communication.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when access to the serial port is not authorized.</exception>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is provided.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="Exception">Thrown for other general errors.</exception>
        public int BaudRate
        {
            get { return _serialPort.BaudRate; }
            set
            {
                try
                {
                    _serialPort.BaudRate = value;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"An error occurred while setting the BaudRate: {ex.Message}");
                }
            }
        }



        /// <summary>
        /// Open the serial port connection.
        /// </summary>
        public void Connect()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Console.WriteLine("Port opened");
                    IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error opening serial port: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                IsConnected = false;
            }
        }


        /// <summary>
        /// Close the serial port connection.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _serialPort.Close();
                Console.WriteLine("Port closed");
                IsConnected = false;
                Disconnected?.Invoke();
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
            _serialPort.WriteTimeout = 5000;

            try
            {
                _serialPort.WriteLine(data);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing to serial port: {ex.Message}");
                IsConnected = false;
                Disconnected?.Invoke();
            }
        }

        /// <summary>
        /// Read data from the serial port.
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            _serialPort.ReadTimeout = 5000;

            try
            {
                string data = _serialPort.ReadLine();
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                Disconnected?.Invoke();
                return null;
            }
        }

        public string ReadAll()
        {
            _serialPort.ReadTimeout = 5000;

            try
            {
                string data = _serialPort.ReadExisting();
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                Disconnected?.Invoke();
                return null;
            }
        }

        public async Task<string> ReadAsync()
        {
            _serialPort.ReadTimeout = 5000;

            try
            {
                string data = await Task.Run(() => _serialPort.ReadLine());
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                Disconnected?.Invoke();
                return null;
            }
        }

        public async Task<string> ReadAllAsync()
        {
            _serialPort.ReadTimeout = 5000;

            try
            {
                string data = await Task.Run(() => _serialPort.ReadExisting());
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                Disconnected?.Invoke();
                return null;
            }
        }


        /// <summary>
        /// Checks to see if printer is busy if this is the case sleeps for 1 second and outputs "busy" to the user then rechecks until it's no longer busy
        /// </summary>
        public bool CheckForBusy()
        {
            string data = _serialPort.ReadLine();

            while (data == _bUSY_CHECK || !HasData())
            {
                Console.WriteLine(_serialPort.ReadLine());
                Thread.Sleep(_sLEEP_TIME_AFTER_BUSY_CHECK);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks on opened port to see if there is data left to read
        /// </summary>
        /// <returns></returns>
        public bool HasData()
        {
            if (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
            {
                IsConnected = true;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Makes a list of available ports
        /// </summary>
        /// <returns></returns>      
        public List<string> AvailableConnections()
        {
            List<string> availablePorts = SerialPort.GetPortNames().ToList();
            return availablePorts;
        }

    }

}
