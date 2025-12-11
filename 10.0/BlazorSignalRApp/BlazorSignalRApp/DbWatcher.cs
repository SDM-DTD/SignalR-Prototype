using BlazorSignalRApp.Hubs;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp.Client;

public class DbWatcher
{
    private const string connectionString = "SERVER=.\\MSSQLSERVER2017;DATABASE=SignalR-Prototype;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true;Connection Timeout=10";
    private readonly IHubContext<ChatHub> _hub;
    private TheData? theData;

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
        GetLatestData();

        _hub.Clients.All.SendAsync("ReceiveData", theData);
        RegisterNotification();
    }

    private void GetLatestData()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT TOP 1 id, value FROM dbo.TheData ORDER BY id DESC", conn);
        conn.Open();
        var latestValue = cmd.ExecuteReader();

        if (latestValue.Read())
        {
            theData = new TheData
            {
                Id = (int)latestValue.GetSqlInt32(0),
                Value = latestValue.GetString(1) ?? string.Empty
            };
        }
    }
}
