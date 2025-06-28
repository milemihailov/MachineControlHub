// WebUIBlazor/Data/PrinterDiscoveryService.cs
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace WebUI.Services
{
    public class PrinterDiscoveryService
    {
        // Common ports used by ESP32 and 3D printers
        private readonly int[] _commonPorts = { 23, 80, 8080, 8000, 3000 };

        public async Task<List<DiscoveredPrinter>> ScanNetworkAsync(string baseIpAddress, CancellationToken cancellationToken = default)
        {
            List<DiscoveredPrinter> discoveredPrinters = new();
            string baseIp = baseIpAddress.Substring(0, baseIpAddress.LastIndexOf('.') + 1);

            var scanTasks = new List<Task<DiscoveredPrinter>>();

            for (int i = 1; i < 255; i++)
            {
                string ip = baseIp + i;
                scanTasks.Add(CheckIpAsync(ip, cancellationToken));
            }

            var results = await Task.WhenAll(scanTasks);
            return results.Where(p => p != null).ToList();
        }

        private async Task<DiscoveredPrinter> CheckIpAsync(string ipAddress, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, 500);

                if (reply.Status == IPStatus.Success)
                {
                    // Try to connect to common printer ports
                    foreach (var port in _commonPorts)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return null;

                        if (await CheckPortAsync(ipAddress, port))
                        {
                            string deviceName = await CheckForESP32Async(ipAddress, port);
                            return new DiscoveredPrinter
                            {
                                IpAddress = ipAddress,
                                Port = port,
                                Name = string.IsNullOrEmpty(deviceName)
                                    ? $"ESP32 at {ipAddress}:{port}"
                                    : deviceName
                            };
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> CheckPortAsync(string ipAddress, int port)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(ipAddress, port);

                if (await Task.WhenAny(connectTask, Task.Delay(500)) == connectTask)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> CheckForESP32Async(string ipAddress, int port)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ipAddress, port);

                using var stream = client.GetStream();

                // First try to send a status command to see if it's our ESP32
                byte[] data = Encoding.ASCII.GetBytes("#STATUS\n");
                stream.Write(data, 0, data.Length);

                // Wait for response
                stream.ReadTimeout = 1000;
                byte[] buffer = new byte[1024];

                Task<int> readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                if (await Task.WhenAny(readTask, Task.Delay(1000)) == readTask)
                {
                    int bytesRead = await readTask;
                    if (bytesRead > 0)
                    {
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        // Check if it looks like our ESP32 response
                        if (response.Contains("ESP32") || response.Contains("Bridge"))
                        {
                            return "ESP32 Serial Bridge";
                        }
                    }
                }

                // If that didn't work, try a standard printer command
                data = Encoding.ASCII.GetBytes("M115\n"); // Firmware info command
                stream.Write(data, 0, data.Length);

                readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                if (await Task.WhenAny(readTask, Task.Delay(1000)) == readTask)
                {
                    int bytesRead = await readTask;
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Look for typical 3D printer firmware identifiers
                    if (response.Contains("FIRMWARE_NAME") ||
                        response.Contains("SKR") ||
                        response.Contains("Marlin"))
                    {
                        return "3D Printer";
                    }
                }

                return "Unknown Device";
            }
            catch
            {
                return "";
            }
        }
    }

    public class DiscoveredPrinter
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
    }
}
