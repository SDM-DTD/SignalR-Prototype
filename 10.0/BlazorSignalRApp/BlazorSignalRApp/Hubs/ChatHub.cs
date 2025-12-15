using DTOs;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub() : Hub<IChatHub>
{
    public async Task SendData(int id, TheData theData)
    {
        await Clients.All.ReceiveData(theData);
    }
}
