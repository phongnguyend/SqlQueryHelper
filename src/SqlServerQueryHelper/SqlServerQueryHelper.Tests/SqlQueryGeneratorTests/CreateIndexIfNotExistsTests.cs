namespace SqlServerQueryHelper.Tests.SqlQueryGeneratorTests;

public class CreateIndexIfNotExistsTests
{
    [Fact]
    public void CreateIndexIfNotExists_WithTableAndName_GeneratesCorrectSql()
    {
        // Arrange
        string tableName = "dbo.Table1";
        string indexName = "IX_Table1_Column1";
        string script = "CREATE INDEX IX_Table1_Column1 ON dbo.Table1 (Column1)";

        // Act
        var sql = SqlQueryGenerator.CreateIndexIfNotExists(tableName, indexName, script);

        // Normalize whitespace for comparison
        var normalizedSql = sql.Replace("\r\n", "\n").Trim();

        var expectedSql = $"""
            IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('{tableName}') AND NAME = '{indexName}')
            BEGIN
                {script};
            END;
            """.Replace("\r\n", "\n").Trim();

        // Assert
        Assert.Equal(expectedSql, normalizedSql);
    }

    [Fact]
    public void CreateIndexIfNotExists_WithoutTableAndName_GeneratesCorrectSql()
    {
        // Arrange
        string tableName = "dbo.Table1";
        string indexName = "IX_Table1_Column1";
        string script = "CREATE INDEX IX_Table1_Column1 ON dbo.Table1 (Column1)";

        // Act
        var sql = SqlQueryGenerator.CreateIndexIfNotExists(script);

        // Normalize whitespace for comparison
        var normalizedSql = sql.Replace("\r\n", "\n").Trim();

        var expectedSql = $"""
            IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('{tableName}') AND NAME = '{indexName}')
            BEGIN
                {script};
            END;
            """.Replace("\r\n", "\n").Trim();

        // Assert
        Assert.Equal(expectedSql, normalizedSql);
    }
}
