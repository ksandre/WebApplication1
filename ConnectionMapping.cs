using StackExchange.Redis;
using System.Threading.Tasks;

public static class ConnectionMapping
{
    private static Lazy<ConnectionMultiplexer>? LazyConnection;
    private static ConnectionMultiplexer Connection => LazyConnection!.Value;
    private static IDatabase Database => Connection.GetDatabase();

    public static void Initialize(string redisConnectionString)
    {
        LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(redisConnectionString));
    }

    public static async Task AddAsync(string paymentId, string connectionId)
    {
        await Database.StringSetAsync(paymentId, connectionId, TimeSpan.FromHours(1));
    }

    public static async Task RemoveAsync(string paymentId)
    {
        await Database.KeyDeleteAsync(paymentId);
    }

    public static async Task<string?> GetConnectionIdAsync(string paymentId)
    {
        var connectionId = await Database.StringGetAsync(paymentId);
        return connectionId.HasValue ? connectionId.ToString() : null;
    }
}
