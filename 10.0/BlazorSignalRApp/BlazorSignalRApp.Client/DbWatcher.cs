using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorSignalRApp.Client;

public class DbWatcher
{
    private const string connectionString = "SERVER=dtdb.database.windows.net;DATABASE=dtechlivedb;UID=dtechUser1;PWD=FELaa^VC8J;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=true;Connection Timeout=10";

    public static void Start()
    {
        SqlDependency.Start(connectionString);
        RegisterNotification();
        Console.WriteLine("Listening for changes. Press Enter to exit...");
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

