using DTOs;

namespace BlazorSignalRApp.Hubs;

public interface IChatHub 
{
    Task ReceiveMessage(string user, string message);

    Task ReceiveData(int id, TheData theData);
}
