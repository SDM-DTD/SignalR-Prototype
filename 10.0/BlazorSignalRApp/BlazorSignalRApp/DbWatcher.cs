using BlazorSignalRApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp.Client;

public class DbWatcher
{
    private const string connectionString = "SERVER=.\\MSSQLSERVER2017;DATABASE=SignalR-Prototype;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true;Connection Timeout=10";
    private readonly IHubContext<ChatHub> _hub;

    public DbWatcher(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public void Start()
    {
        SqlDependency.Start(connectionString);
        RegisterNotification();
        Console.WriteLine("Listening for changes. Press Enter to exit...");
        //      SqlDependency.Stop(connectionString);
    }

    public void RegisterNotification()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT value FROM dbo.TheData", conn);
        var dependency = new SqlDependency(cmd);
        dependency.OnChange += OnDatabaseChange;
        conn.Open();
        cmd.ExecuteReader(); // Start listening
    }

    private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
    {
        Console.WriteLine($"Database change detected: {e.Type}, {e.Info}, {e.Source}");
        _hub.Clients.All.SendAsync("ReceiveMessage", "user", "message");
        RegisterNotification();
    }
}
