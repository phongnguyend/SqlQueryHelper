using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SqlServerQueryHelper.EntityFrameworkCore;

public class SqlQueryExecutor
{
    public static void ExecuteSql(string path, string connectionString, Action<string> log = null)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.sql").OrderBy(f => f))
        {
            log?.Invoke($"Executing File: {Path.GetFileName(file)}");

            string script = File.ReadAllText(file);

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            foreach (var sql in SqlQueryParser.SplitSqlBatches(script))
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    continue;
                }

                log?.Invoke($"Executing Batch:");
                log?.Invoke(sql);

                using var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }

    public static void ExecuteSql(string path, DbContext dbContext, Action<string> log = null)
    {
        ExecuteSql(path, dbContext.Database.GetConnectionString()!, log);
    }
}
