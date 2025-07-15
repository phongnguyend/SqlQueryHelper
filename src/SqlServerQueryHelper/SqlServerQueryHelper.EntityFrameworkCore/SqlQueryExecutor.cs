using Microsoft.Data.SqlClient;

namespace SqlServerQueryHelper.EntityFrameworkCore;

public class SqlQueryExecutor
{
    public static void ExecuteSql(string path, string connectionString)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.sql").OrderBy(f => f))
        {
            Console.WriteLine($"Executing: {Path.GetFileName(file)}");

            string script = File.ReadAllText(file);

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            foreach (var sql in SqlQueryParser.SplitSqlBatches(script))
            {
                using var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
