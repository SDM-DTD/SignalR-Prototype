using Microsoft.Data.SqlClient;

namespace BlazorSignalRApp;

public class DbWatcher
{
    private const string connectionString = "Server=(localdb)\\MSSQLocalDB;Database=SignalR-Prototype;Trusted_Connection=True;";

    public static void Start()
    {
        SqlDependency.Start(connectionString);
        RegisterNotification();
        Console.WriteLine("Listening for changes. Press Enter to exit...");
        Console.ReadLine();
        SqlDependency.Stop(connectionString);
    }

    public static void RegisterNotification()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT value FROM dbo.TheData", conn);
        var dependency = new SqlDependency(cmd);
        dependency.OnChange += OnDatabaseChange;
        conn.Open();
        cmd.ExecuteReader(); // Start listening
    }

    private static void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
    {
        Console.WriteLine($"Database change detected: {e.Type}, {e.Info}, {e.Source}");
        RegisterNotification();
    }
}

