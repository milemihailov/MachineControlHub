
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MachineControlHub.PrinterConnection
{
    public class WiFiConnection : IPrinterConnection
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private object _readLock = new object();
        private Timer _readTimer;

        public string IpAddress { get; set; } = "192.168.0.1";
        public int Port { get; set; } = 23; // Default Telnet port

        public bool IsConnected { get; set; }
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

        }

        public void Write(string message)
        {

        }

        public string Read()
        {
            return null;
        }

        public string ReadAll()
        {
            return null;
        }

        public List<string> AvailableConnections()
        {
            return new List<string>();
        }

        bool IPrinterConnection.HasData()
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadAllAsync()
        {
            throw new NotImplementedException();
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
    }
}
