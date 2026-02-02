using BlazorSignalRApp.Hubs;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp;

public class DbWatcher(IHubContext<ChatHub>? hub) : IDisposable
{
    private const string connectionString = "Data Source=DTD-LT71\\MSSQLSERVER02;DATABASE=SignalR-Prototype;Integrated Security=True;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true;Connection Timeout=10";

    public void Start()
    {
        SqlDependency.Start(connectionString);
        RegisterNotification();
    }

    public void RegisterNotification()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT [Id] FROM dbo.TheData", conn);
        var dependency = new SqlDependency(cmd);
        dependency.OnChange += OnDatabaseChange;
        conn.Open();
        cmd.ExecuteReader().Close();
    }

    private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
    {
        var theData  = GetLatestData();

        if (hub != null && theData != null)
        {
            _ = ChatHub.NotifyClientsAsync(hub, theData.Id.ToString(), theData);
        }

        RegisterNotification();
    }

    private static TheData? GetLatestData()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT TOP 1 * FROM [SignalR-Prototype].[dbo].[TheData] ORDER BY [Id] DESC", conn);
        conn.Open();
        var latestValue = cmd.ExecuteReader();

        if (latestValue.HasRows && latestValue.Read())
        {
            return new TheData
            {
                Id = latestValue.GetInt32(0),
                Value = latestValue.GetString(1),
                RunId = latestValue.GetInt32(2)
            };
        }

        return null;
    }

    public void Dispose()
    {
        SqlDependency.Stop(connectionString);
    }
}
