using DTOs;

namespace BlazorSignalRApp.Hubs;

public interface IChatHub 
{
    Task ReceiveData(TheData theData);
}
