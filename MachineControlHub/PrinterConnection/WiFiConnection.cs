
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MachineControlHub.PrinterConnection
{
    public class WiFiConnection : IPrinterConnection
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private object _readLock = new object();
        private Timer _readTimer;
        public bool EnableVerboseLogging { get; set; } = true;
        public int ReadTimeout { get; set; } = 5000; // Default read timeout in milliseconds
        public string IpAddress { get; set; } = "192.168.0.1";
        public int Port { get; set; } = 23; // Default Telnet port

        public bool IsConnected { get; set; }
        public int ResponseDelay { get; set; } = 100; // Delay after sending a command before reading response
        public event EventHandler<string> DataReceived;

        public WiFiConnection()
        {
            _client = new TcpClient();
        }

        public void Initialize(string connectionString)
        {
            try
            {
                string[] parts = connectionString.Split(',');

                if (parts.Length < 2)
                    throw new ArgumentException("WiFi connection string must be in format: 'IP_ADDRESS,PORT'");

                IpAddress = parts[0].Trim();

                if (!int.TryParse(parts[1].Trim(), out int port))
                    throw new ArgumentException("Invalid port format");

                Port = port;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error initializing WiFi connection", ex);
            }
        }

        public void Connect()
        {
            try
            {
                if (IsConnected)
                    return;

                _client = new TcpClient();
                _client.SendTimeout = 5000;
                _client.ReceiveTimeout = 5000;
                _client.Connect(IpAddress, Port);
                _stream = _client.GetStream();
                IsConnected = true;

                // Start background reading
                StartBackgroundReading();

                Console.WriteLine($"Connected to printer at {IpAddress}:{Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
                IsConnected = false;
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                StopBackgroundReading();

                // Close the stream and client
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                    _stream = null;
                }

                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }
                IsConnected = false;
                Console.WriteLine("Disconnected from printer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disconnect: {ex.Message}");
                IsConnected = false;

                // Even if there's an error, ensure we clean up
                _stream = null;
                _client = null;
                IsConnected = false;
            }
        }

        public void Write(string message)
        {
            if (!IsConnected || _stream == null)
                throw new InvalidOperationException("Not connected.");

            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to printer: {ex.Message}");
                IsConnected = false;
                throw;
            }
        }

        public string Read()
        {
            
            if (!IsConnected || _stream == null)
                throw new InvalidOperationException("Not connected.");
            lock (_readLock)
            {
                try
                {
                    if (_stream.DataAvailable)
                    {
                        using (var reader = new StreamReader(_stream, System.Text.Encoding.UTF8, true, 1024, true))
                        {
                            return reader.ReadToEnd();
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading from printer: {ex.Message}");
                    IsConnected = false;
                    return null;
                }
            }
        }

        public string ReadAll()
        {
            if (!IsConnected || _stream == null)
                return null;

            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buffer = new byte[4096];

                while (_stream.DataAvailable)
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        sb.Append(data);
                    }
                    else
                    {
                        break;
                    }
                }

                string result = sb.Length > 0 ? sb.ToString() : null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from printer: {ex.Message}");
                return null;
            }
        }

        public List<string> AvailableConnections()
        {
            return new List<string> { "Manual WiFi Connection" };
        }

        bool IPrinterConnection.HasData()
        {
            if (!IsConnected || _stream == null)
                return false;
            lock (_readLock)
            {
                try
                {
                    return _stream.DataAvailable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking data availability: {ex.Message}");
                    IsConnected = false;
                    return false;
                }
            }
        }

        public async Task<string> ReadAsync()
        {
            if (!IsConnected || _stream == null)
                return null;

            try
            {
                byte[] buffer = new byte[1024];

                if (_stream.DataAvailable)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        return data;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from printer: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ReadAllAsync()
        {
            if (!IsConnected || _stream == null)
                return null;

            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buffer = new byte[4096];

                while (_stream.DataAvailable)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        sb.Append(data);
                    }
                    else
                    {
                        break;
                    }
                }

                string result = sb.Length > 0 ? sb.ToString() : null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from printer: {ex.Message}");
                return null;
            }
        }

        private void StartBackgroundReading()
        {
            StopBackgroundReading();

            _readTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    if (IsConnected && _stream != null && _stream.DataAvailable)
                    {
                        lock (_readLock)
                        {
                            string data = ReadAll();
                            if (!string.IsNullOrEmpty(data))
                            {
                                DataReceived?.Invoke(this, data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Background read error: {ex.Message}");
                }
            }, null, 0, 50); // Check every 50ms
        }

        private void StopBackgroundReading()
        {
            if (_readTimer != null)
            {
                _readTimer.Dispose();
                _readTimer = null;
            }
        }


        public string ReadWithTimeout(int timeoutMs = -1)
        {
            if (timeoutMs < 0) timeoutMs = ReadTimeout;

            if (!IsConnected || _stream == null)
                return null;

            try
            {
                DateTime startTime = DateTime.Now;
                StringBuilder sb = new StringBuilder();
                bool receivedData = false;

                while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
                {
                    if (_stream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            sb.Append(data);
                            receivedData = true;
                        }
                    }

                    // If we've received some data, wait a short while for more
                    // Otherwise, check more frequently
                    Thread.Sleep(receivedData ? 50 : 10);

                    // If we have data and haven't received more in the last 100ms, we're probably done
                    if (receivedData && !_stream.DataAvailable)
                    {
                        Thread.Sleep(100);
                        if (!_stream.DataAvailable)
                            break;
                    }
                }

                string result = sb.ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine($"ReadWithTimeout result: {result}");
                    return result;
                }

                if (EnableVerboseLogging)
                    Console.WriteLine($"Timeout after {timeoutMs}ms waiting for data");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadWithTimeout: {ex.Message}");
                return null;
            }
        }

        // Command-response pattern with timeout
        public string SendCommandWithResponse(string command, int timeoutMs = 3000)
        {
            if (!IsConnected || _stream == null)
                throw new InvalidOperationException("Not connected to the printer");

            try
            {
                // Clear any pending data first
                while (_stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    _stream.Read(buffer, 0, buffer.Length);
                }

                // Send the command
                Write(command);

                // Wait a moment for the response to start arriving
                Thread.Sleep(ResponseDelay);

                // Read with timeout
                return ReadWithTimeout(timeoutMs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendCommandWithResponse: {ex.Message}");
                return null;
            }
        }
    }
}
