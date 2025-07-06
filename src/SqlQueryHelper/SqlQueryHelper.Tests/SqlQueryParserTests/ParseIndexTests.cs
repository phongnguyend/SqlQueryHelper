namespace SqlQueryHelper.Tests.SqlQueryParserTests;

public class ParseIndexTests
{
    [Fact]
    public void ParseIndex()
    {
        // Arrange
        var sql = """
            CREATE NONCLUSTERED INDEX [IDX_Name]
            ON [dbo].[TableName] ([Col1],[Col2],[Col3],[Col4])
            """;

        // Act
        var indexInfo = SqlQueryParser.ParseIndex(sql);

        Assert.Equal("[IDX_Name]", indexInfo.Name);
        Assert.Equal("[dbo].[TableName]", indexInfo.TableName);
    }
}