
namespace MachineControlHub.PrinterConnection
{
    public class WiFiConnection : IPrinterConnection
    {
        bool IPrinterConnection.IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string connectionString)
        {

        }

        public void Connect()
        {

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
    }
}
