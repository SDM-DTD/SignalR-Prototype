using DTOs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> subscriptions = new();

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        subscriptions.TryRemove(Context.ConnectionId, out _);

        return base.OnDisconnectedAsync(exception);
    }

    public Task Subscribe(int runId)
    {
        if (runId > 0)
        {
            subscriptions[Context.ConnectionId] = runId.ToString();
        }

        return Task.CompletedTask;
    }

    public static async Task NotifyClientsAsync(IHubContext<ChatHub> hub, string keyId, TheData theData)
    {
        var clientConnections = subscriptions.Where(x => x.Value == keyId).Select(x => x.Key).ToList();

        foreach (var clientConnection in clientConnections)
        {
            await hub.Clients.Client(clientConnection).SendAsync("ReceiveData", theData);
        }
    }
}
