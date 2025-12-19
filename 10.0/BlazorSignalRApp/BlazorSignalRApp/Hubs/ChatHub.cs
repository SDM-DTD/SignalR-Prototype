using System.ClientModel.Primitives;
using DTOs;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub(ClientManager clientManagaer) : Hub<IChatHub>
{
    public async Task SendData(int id, TheData theData)
    {
        await Clients.All.ReceiveData(theData);
    }

    public void SetClientId(int id, string connectionId)
    {
        clientManagaer.AddClient(id, connectionId);
    }

    public int GetId()
    {
        return clientManagaer.GetConnectionId(Context.ConnectionId) ?? 0;
    }

    public override async Task OnConnectedAsync()
    {
        clientManagaer.AddClient(GetId(), Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
