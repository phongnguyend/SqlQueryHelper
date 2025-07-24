using Microsoft.Data.SqlClient;
using System.Text;

namespace SqlServerQueryHelper.EntityFrameworkCore;

public class SqlQueryExecutor
{
    private static readonly bool DebugEnabled = false;

    public static void ExecuteSqlFiles(string path, string connectionString, Action<string> log = null)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.sql").OrderBy(f => f))
        {
            ExecuteSqlFile(file, connectionString, log);
        }
    }

    public static void ExecuteSqlFile(string file, string connectionString, Action<string> log = null)
    {
        log?.Invoke($"Executing File: {Path.GetFileName(file)}");

        string script = File.ReadAllText(file);

        StringBuilder test = null;

        if (DebugEnabled)
        {
            test = new StringBuilder();
        }

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var sql in SqlQueryParser.SplitSqlBatches(script))
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                continue;
            }

            if (DebugEnabled)
            {
                test!.AppendLine(sql);
            }

            log?.Invoke($"Executing Batch:");
            log?.Invoke(sql);

            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        if (DebugEnabled)
        {
            File.WriteAllText(file + ".debug", test!.ToString());
        }
    }
}
