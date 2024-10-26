using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    // Method for clients to send messages, which will then be broadcasted to all other clients
    public async Task SendMessage(string message)
    {
        // Broadcasts the message to all connected clients
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var paymentId = httpContext.Request.Query["paymentId"].ToString();

        // Store the userId and connectionId in ConnectionMapping
        ConnectionMapping.Add(paymentId, Context.ConnectionId);

        await base.OnConnectedAsync();

        System.Diagnostics.Debug.WriteLine($"ConnectionId {paymentId}/{Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var httpContext = Context.GetHttpContext();
        var paymentId = httpContext.Request.Query["paymentId"].ToString();

        // Remove the userId and connectionId from ConnectionMapping
        ConnectionMapping.Remove(paymentId);

        await base.OnDisconnectedAsync(exception);

    }
}