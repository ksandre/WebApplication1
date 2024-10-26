using System.Collections.Concurrent;

public static class ConnectionMapping
{
    private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

    public static void Add(string paymentId, string connectionId)
    {
        _connections[paymentId] = connectionId;
    }

    public static void Remove(string paymentId)
    {
        _connections.TryRemove(paymentId, out _);
    }

    public static string? GetConnectionId(string paymentId)
    {
        return _connections.TryGetValue(paymentId, out string? connectionId) ? connectionId : null;
    }
}