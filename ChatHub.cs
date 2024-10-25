using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    // Method for clients to send messages, which will then be broadcasted to all other clients
    public async Task SendMessage(string user, string message)
    {
        // Broadcasts the message to all connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}