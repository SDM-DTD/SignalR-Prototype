using BlazorSignalRApp.Hubs;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp.Client;

public class DbWatcher
{
    private const string connectionString = "Data Source=DTD-LT71\\MSSQLSERVER02;DATABASE=SignalR-Prototype;Integrated Security=True;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true;Connection Timeout=10";
    //private const string connectionString = "SERVER=.\\MSSQLSERVER2017;DATABASE=SignalR-Prototype;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true;Connection Timeout=10";
    private readonly IHubContext<ChatHub>? _hub;
    private readonly ClientManager _clientManager;
    private TheData? theData;

    public DbWatcher(IHubContext<ChatHub>? hub, ClientManager clientManager)
    {
        _hub = hub;
        _clientManager = clientManager;
    }

    public void Start()
    {
        SqlDependency.Start(connectionString);
        RegisterNotification();
        Console.WriteLine("Listening for changes. Press Enter to exit...");
//        SqlDependency.Stop(connectionString);
    }

    public void RegisterNotification()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT [value] FROM dbo.TheData", conn);
        var dependency = new SqlDependency(cmd);
        dependency.OnChange += OnDatabaseChange;
        conn.Open();
        cmd.ExecuteReader();
    }

    private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
    {
        Console.WriteLine($"Database change detected: {e.Type}, {e.Info}, {e.Source}");
        GetLatestData();
        _hub.Clients.All.SendAsync("ReceiveData", theData);
        RegisterNotification();
    }

    private void GetLatestData()
    {
        //var id = _clientManager.GetConnectionId()
        id = 242;
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand($"SELECT TOP 1 [id], [Value] FROM [SignalR-Prototype].[dbo].[TheData] WHERE [id]={id} ORDER BY [Id] DESC", conn);
        conn.Open();
        var latestValue = cmd.ExecuteReader();

        if (latestValue.Read() && latestValue.HasRows)
        {
            theData = new TheData
            {
                Id = (int)latestValue.GetInt32(0),
                Value = latestValue.GetString(1)
            };
        }
    }
}
