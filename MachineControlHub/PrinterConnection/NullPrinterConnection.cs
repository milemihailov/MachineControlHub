using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineControlHub.PrinterConnection;

public class NullPrinterConnection : IPrinterConnection
{
    public bool IsConnected { get; set; } = false;

    public event EventHandler<string> DataReceived;

    public void Connect() { }
    public void Disconnect() { }
    public void Write(string command) { }
    public string ReadAll() => string.Empty;
    public bool HasData() => false;
    public void Initialize(string connectionString) { }

    public string Read() => string.Empty;

    public Task<string> ReadAsync() => Task.FromResult(string.Empty);

    public Task<string> ReadAllAsync() => Task.FromResult(string.Empty);

    public List<string> AvailableConnections() => new List<string>();
}
