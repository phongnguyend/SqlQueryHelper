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
            INCLUDE([Col5],[Col6],[Col7],[Col8],[Col9],[Col10],[Col11])
            WITH (ONLINE = ON)
            """;

        // Act
        var indexInfo = SqlQueryParser.ParseIndex(sql);

        Assert.Equal("[IDX_Name]", indexInfo.Name);
        Assert.Equal("[dbo].[TableName]", indexInfo.TableName);
    }

    [Fact]
    public void ParseIndexV2_CleanSpacing()
    {
        // Arrange
        var sql = """
            CREATE NONCLUSTERED INDEX [IDX_Name]
            ON [dbo].[TableName] ([Col1],[Col2],[Col3],[Col4])
            INCLUDE ([Col5],[Col6],[Col7],[Col8],[Col9],[Col10],[Col11])
            WITH (ONLINE = ON)
            """;

        // Act
        var indexInfo = SqlQueryParser.ParseIndexV2(sql);

        Assert.Equal("[IDX_Name]", indexInfo.Name);
        Assert.Equal("[dbo].[TableName]", indexInfo.TableName);
    }

    [Fact]
    public void ParseIndexV2_BadSpacing()
    {
        // Arrange
        var sql = """
            CREATE NONCLUSTERED INDEX[IDX_Name]
            ON[dbo].[TableName]([Col1],[Col2],[Col3],[Col4])
            INCLUDE([Col5],[Col6],[Col7],[Col8],[Col9],[Col10],[Col11])
            WITH (ONLINE = ON)
            """;

        // Act
        var indexInfo = SqlQueryParser.ParseIndexV2(sql);

        Assert.Equal("[IDX_Name]", indexInfo.Name);
        Assert.Equal("[dbo].[TableName]", indexInfo.TableName);
    }

    [Fact]
    public void ParseIndexV2_NameHasSpecialCharacters()
    {
        // Arrange
        var sql = """
            CREATE NONCLUSTERED INDEX[IDX_Name]
            ON[dbo].[[[Table]]Name]([Col1],[Col2],[Col3],[Col4])
            INCLUDE([Col5],[Col6],[Col7],[Col8],[Col9],[Col10],[Col11])
            WITH (ONLINE = ON)
            """;

        // Act
        var indexInfo = SqlQueryParser.ParseIndexV2(sql);

        Assert.Equal("[IDX_Name]", indexInfo.Name);
        Assert.Equal("[dbo].[[[Table]]Name]", indexInfo.TableName);
    }
}