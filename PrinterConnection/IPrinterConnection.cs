namespace ControllingAndManagingApp.Connection
{
    /// <summary>
    /// Represents an interface for printer connections.
    /// </summary>
    public interface IPrinterConnection
    {
        /// <summary>
        /// Initializes the printer connection based on the provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string containing information about the printer connection.</param>
        void Initialize(string connectionString);

        /// <summary>
        /// Connects to the printer.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the printer.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Writes a message to the printer.
        /// </summary>
        /// <param name="message">The message to be written to the printer.</param>
        void Write(string message);

        /// <summary>
        /// Reads a message from the printer.
        /// </summary>
        /// <returns>The message read from the printer.</returns>
        string Read();

        /// <summary>
        /// Retrieves a list of available printer connections.
        /// </summary>
        /// <returns>A list of available printer connections.</returns>
        List<string> AvailableConnections();
    }
}
