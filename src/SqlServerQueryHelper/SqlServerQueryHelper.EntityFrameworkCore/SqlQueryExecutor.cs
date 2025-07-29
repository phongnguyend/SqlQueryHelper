using Microsoft.Data.SqlClient;
using System.Text;

namespace SqlServerQueryHelper.EntityFrameworkCore;

public class SqlQueryExecutor
{
    private static readonly bool DebugEnabled = false;

    public static void ExecuteSqlFiles(string path, ExecutionContext context)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.sql").OrderBy(f => f))
        {
            ExecuteSqlFile(file, context);
        }
    }

    public static void ExecuteSqlFile(string file, ExecutionContext context)
    {
        context!.LogTo?.Invoke($"Executing File: {Path.GetFileName(file)}");

        string script = File.ReadAllText(file);

        StringBuilder test = null;

        if (DebugEnabled)
        {
            test = new StringBuilder();
        }

        using var connection = new SqlConnection(context!.ConnectionString);
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

            context?.LogTo?.Invoke($"Executing Batch:");
            context?.LogTo?.Invoke(sql);

            using var command = new SqlCommand(sql, connection);

            if (context!.CommandTimeout.HasValue)
            {
                command.CommandTimeout = context.CommandTimeout.Value;
            }

            command.ExecuteNonQuery();
        }

        if (DebugEnabled)
        {
            File.WriteAllText(file + ".debug", test!.ToString());
        }
    }
}

public class ExecutionContext
{
    public string ConnectionString { get; set; }

    public int? CommandTimeout { get; set; }

    public Action<string> LogTo { get; set; }
}