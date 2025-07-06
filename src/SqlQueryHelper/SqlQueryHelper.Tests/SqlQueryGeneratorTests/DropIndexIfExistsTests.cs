namespace SqlQueryHelper.Tests.SqlQueryGeneratorTests;

public class DropIndexIfExistsTests
{
    [Theory]
    [InlineData("dbo.Table1", "IX_Table1_Column1", "IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.Table1') AND NAME = 'IX_Table1_Column1')\nBEGIN\n    DROP INDEX [IX_Table1_Column1] ON dbo.Table1\nEND;")]
    [InlineData(" [dbo].[Table2] ", "[IX_Table2_Column2]", "IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('[dbo].[Table2]') AND NAME = 'IX_Table2_Column2')\nBEGIN\n    DROP INDEX [IX_Table2_Column2] ON [dbo].[Table2]\nEND;")]
    [InlineData("Table3", "IX_Table3_Col3", "IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('Table3') AND NAME = 'IX_Table3_Col3')\nBEGIN\n    DROP INDEX [IX_Table3_Col3] ON Table3\nEND;")]
    public void DropIndexIfExists_GeneratesCorrectSql(string tableName, string indexName, string expectedSql)
    {
        // Act
        var sql = SqlQueryGenerator.DropIndexIfExists(tableName, indexName);

        // Normalize whitespace for comparison
        var normalizedSql = sql.Replace("\r\n", "\n").Trim();
        var normalizedExpected = expectedSql.Replace("\r\n", "\n").Trim();

        // Assert
        Assert.Equal(normalizedExpected, normalizedSql);
    }
}