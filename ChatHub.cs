using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public sealed class ChatHub : Hub
{
    public async Task SendMessage(string content)
    {
        await Clients.All.SendAsync("ReceiveMessage", content);
    }
}