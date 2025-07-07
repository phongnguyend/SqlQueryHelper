using System.Text;

namespace SqlQueryHelper;

public class SqlQueryGenerator
{
    public static string CreateIndexIfNotExists(string tableName, string indexName, string script)
    {
        tableName = tableName.Trim();
        indexName = indexName.Trim().Trim('[', ']');

        string sql = $"""
            IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('{tableName}') AND NAME = '{indexName}')
            BEGIN
                {script};
            END;
            """;

        return sql;
    }

    public static string DropIndexIfExists(string tableName, string indexName)
    {
        tableName = tableName.Trim();
        indexName = indexName.Trim().Trim('[', ']');

        string sql = $"""
            IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('{tableName}') AND NAME = '{indexName}')
            BEGIN
                DROP INDEX [{indexName}] ON {tableName}
            END;
            """;

        return sql;
    }

    public static string DeleteDuplicatedRecords(string tableName, string[] duplicatedColumns, string[] orderByColumns, string id)
    {
        string autoColumn = Guid.NewGuid().ToString();

        var sqlBuilder = new StringBuilder();

        sqlBuilder.AppendLine("DELETE T");
        sqlBuilder.AppendLine("FROM");
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"SELECT [{id}], [{autoColumn}] = ROW_NUMBER() OVER (");
        sqlBuilder.AppendLine($"              PARTITION BY {string.Join(", ", duplicatedColumns.Select(x => $"[{x}]"))}");
        sqlBuilder.AppendLine($"              ORDER BY {string.Join(", ", orderByColumns.Select(x => $"[{x}]"))}");
        sqlBuilder.AppendLine("            )");
        sqlBuilder.AppendLine($"FROM {tableName}");
        sqlBuilder.AppendLine(") AS T");
        sqlBuilder.AppendLine($"WHERE [{autoColumn}] > 1");

        var sql = sqlBuilder.ToString();
        return sql;
    }

    public static string CountDuplicatedRecords(string tableName, string[] columns)
    {
        var sqlBuilder = new StringBuilder();

        sqlBuilder.AppendLine($"SELECT {string.Join(", ", columns.Select(x => $"[{x}]"))}, COUNT(*) AS COUNT");
        sqlBuilder.AppendLine($"FROM {tableName}");
        sqlBuilder.AppendLine($"GROUP BY {string.Join(", ", columns.Select(x => $"[{x}]"))}");
        sqlBuilder.AppendLine($"HAVING COUNT(*) > 1");

        var sql = sqlBuilder.ToString();
        return sql;
    }
}
