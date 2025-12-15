using BlazorSignalRApp.Hubs;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp.Client;

public class DbWatcher
{
    private const string connectionString = "SERVER=.\\MSSQLSERVER2017;DATABASE=SignalR-Prototype;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true;Connection Timeout=10";
    private readonly IHubContext<ChatHub>? _hub;
    private TheData _theData;

    public DbWatcher(IHubContext<ChatHub>? hub, TheData theData)
    {
        _hub = hub;
        _theData = theData;
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
        using var cmd = new SqlCommand("SELECT [value] FROM dbo.TheData", conn);
        var dependency = new SqlDependency(cmd);
        dependency.OnChange += OnDatabaseChange;
        conn.Open();
        cmd.ExecuteReader(); // Start listening
    }

    private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
    {
        Console.WriteLine($"Database change detected: {e.Type}, {e.Info}, {e.Source}");
        GetLatestData();
        _hub.Clients.All.SendAsync("ReceiveData", _theData);
        RegisterNotification();
    }

    private void GetLatestData()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand($"SELECT TOP 1 id, [value] FROM dbo.TheData WHERE id={_theData.Id} ORDER BY id DESC", conn);
        conn.Open();
        var latestValue = cmd.ExecuteReader();

        if (latestValue.Read() && latestValue.HasRows)
        {            
            _theData.Id = (int)latestValue.GetInt32(0);
            _theData.Value = latestValue.GetString(1);            
        }
    }
}
