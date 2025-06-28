using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MachineControlHub.PrinterConnection;

public static class PrinterConnectionFactory
{

    public static IPrinterConnection CreateConnection(string connectionType, string connectionString)
    {
        IPrinterConnection connection = connectionType.ToLower() switch
        {
            "serial" => new SerialConnection(),
            "wifi" => new WiFiConnection(),
            _ => throw new ArgumentException($"Unsupported connection type: {connectionType}")
        };

        connection.Initialize(connectionString);
        return connection;
    }

    public static string GetConnectionName(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return connectionString;

        string[] parts = connectionString.Split(',');
        string portOrIp = parts[0].Trim();

        // Check if the connection string is for a serial port
        if (IsSerialConnectionString(portOrIp))
        {
            // Return just the port name without any baud rate
            return portOrIp;
        }

        // Check if the connection string is for WiFi
        if (IsWiFiConnectionString(connectionString))
        {
            // Return just the IP address without the port
            return portOrIp;
        }

        // Default to the connection string itself
        return connectionString;
    }

    // Determine if a connection string appears to be for WiFi
    public static bool IsWiFiConnectionString(string connectionString)
    {
        if (connectionString == null) return false;

        // If it contains dots like an IP address and matches IP pattern
        var ipPattern = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
        return Regex.IsMatch(connectionString.Split(',')[0].Trim(), ipPattern);
    }

    // Determine if a connection string is for a serial port
    public static bool IsSerialConnectionString(string portName)
    {
        if (string.IsNullOrEmpty(portName)) return false;

        // Windows: COM1, COM2, etc.
        if (portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            return true;

        // Linux: /dev/ttyUSB0, /dev/ttyACM0, etc.
        if (portName.StartsWith("/dev/tty", StringComparison.OrdinalIgnoreCase))
            return true;

        // macOS: /dev/cu.usbmodem, /dev/cu.usbserial, etc.
        if (portName.StartsWith("/dev/cu.", StringComparison.OrdinalIgnoreCase))
            return true;

        // macOS alternate format: /dev/tty.usbmodem, etc.
        if (portName.StartsWith("/dev/tty.", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
