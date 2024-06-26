﻿using MachineControlHub.LogErrorHistory;
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
        const string BUSY_CHECK = "echo:busy: processing\n";
        const int SLEEP_TIME_AFTER_BUSY_CHECK = 1500;

        private SerialPort serialPort;
        public bool IsConnected { get; set; }
        public SerialConnection()
        {
            serialPort = new SerialPort();
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = true;
        }

        public void Initialize(string connectionString)
        {
            string[] connectionParams = connectionString.Split(',');
            string portName = connectionParams[0].Trim();
            int baudRate = int.Parse(connectionParams[1].Trim());

            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
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
            get { return serialPort.PortName; }
            set
            {
                try
                {
                    serialPort.PortName = value;
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
            get { return serialPort.BaudRate; }
            set
            {
                try
                {
                    serialPort.BaudRate = value;
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
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
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
                serialPort.Close();
                Console.WriteLine("Port closed");
                IsConnected = false;
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
            catch (Exception ex)
            {
                Logger.LogError($"Error writing to serial port: {ex.Message}");
                //IsConnected = false;
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
                string data = serialPort.ReadLine();
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                return null;
            }
        }

        public string ReadAll()
        {
            serialPort.ReadTimeout = 5000;

            try
            {
                string data = serialPort.ReadExisting();
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                IsConnected = false;
                return null;
            }
        }

        public async Task<string> ReadAsync()
        {
            serialPort.ReadTimeout = 5000;

            try
            {
                string data = await Task.Run(() => serialPort.ReadLine());
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading from serial port: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ReadAllAsync()
        {
            serialPort.ReadTimeout = 5000;

            try
            {
                string data = await Task.Run(() => serialPort.ReadExisting());
                return data;
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
        public bool CheckForBusy()
        {
            string data = serialPort.ReadLine();

            while (data == BUSY_CHECK || !HasData())
            {
                Console.WriteLine(serialPort.ReadLine());
                Thread.Sleep(SLEEP_TIME_AFTER_BUSY_CHECK);
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
        public List<string> AvailableConnections()
        {
            List<string> availablePorts = SerialPort.GetPortNames().ToList();
            return availablePorts;
        }

    }

}
