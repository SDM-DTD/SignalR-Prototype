namespace BlazorSignalRApp;

public class ClientManager
{
    private readonly Dictionary<string, int> clientConnections = new();

    public void AddClient(int id, string connectionId)
    {
        clientConnections[connectionId] = id;
    }

    public int? GetConnectionId(string connectionId)
    {
        clientConnections.TryGetValue(connectionId, out var id);

        return id;
    }
}
