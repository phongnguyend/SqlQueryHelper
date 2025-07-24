using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SqlServerQueryHelper.EntityFrameworkCore;

public static class RelationalDatabaseFacadeExtensions
{
    public static string? GetMasterDbConnectionString(this DatabaseFacade databaseFacade)
    {
        return databaseFacade.GetConnectionString("master");
    }

    public static string? GetConnectionString(this DatabaseFacade databaseFacade, string dbName)
    {
        return SwitchDatabase(databaseFacade.GetConnectionString()!, dbName);
    }

    public static string SwitchDatabase(string connectionString, string dbName)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = dbName
        };

        var newConnectionString = connectionStringBuilder.ToString();

        return newConnectionString;
    }

    public static string? GetServerEdition(this DatabaseFacade databaseFacade)
    {
        string connectionString = databaseFacade.GetConnectionString("master")!;

        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT SERVERPROPERTY('Edition')";
        var edition = command.ExecuteScalar()?.ToString();
        return edition;
    }

    public static void ExecuteSqlFiles(this DatabaseFacade databaseFacade, string path, Action<string> log = null)
    {
        SqlQueryExecutor.ExecuteSqlFiles(path, databaseFacade.GetConnectionString()!, log);
    }

    public static void ExecuteSqlFile(this DatabaseFacade databaseFacade, string file, Action<string> log = null)
    {
        SqlQueryExecutor.ExecuteSqlFile(file, databaseFacade.GetConnectionString()!, log);
    }
}
