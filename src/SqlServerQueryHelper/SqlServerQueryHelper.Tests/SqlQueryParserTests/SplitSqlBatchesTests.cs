namespace SqlServerQueryHelper.Tests.SqlQueryParserTests;

public class SplitSqlBatchesTests
{
    [Theory]
    [InlineData("SELECT 1\nGO\nSELECT 2", new[] { "SELECT 1", "SELECT 2" })]
    [InlineData("SELECT 1\r\nGO\r\nSELECT 2", new[] { "SELECT 1", "SELECT 2" })]
    [InlineData("SELECT 1\nGO\nSELECT 2\nGO\nSELECT 3", new[] { "SELECT 1", "SELECT 2", "SELECT 3" })]
    [InlineData("SELECT 1", new[] { "SELECT 1" })]
    [InlineData("GO", new string[] { })]
    [InlineData("SELECT 1\nGO\nGO\nSELECT 2", new[] { "SELECT 1", "SELECT 2" })]
    public void SplitSqlBatches_SplitsCorrectly(string script, string[] expectedBatches)
    {
        var batches = SqlQueryParser.SplitSqlBatches(script);
        Assert.Equal(expectedBatches, batches);
    }

    [Fact]
    public void SplitSqlBatches_HandlesEmptyInput()
    {
        var batches = SqlQueryParser.SplitSqlBatches("");
        Assert.Empty(batches);
    }

    [Fact]
    public void SplitSqlBatches_HandlesOnlyGoSeparators()
    {
        var script = "GO\nGO\nGO";
        var batches = SqlQueryParser.SplitSqlBatches(script);
        Assert.Empty(batches);
    }

    [Fact]
    public void SplitSqlBatches_HandlesCommentsAndWhitespace()
    {
        var script = "SELECT 1 -- comment\nGO\n/* multi-line\ncomment */\nSELECT 2\nGO";
        var batches = SqlQueryParser.SplitSqlBatches(script);
        Assert.Equal(new[] { "SELECT 1 -- comment", "/* multi-line\ncomment */\nSELECT 2" }, batches);
    }

    [Fact]
    public void SplitSqlBatches_HandlesTrailingGo()
    {
        var script = "SELECT 1\nGO";
        var batches = SqlQueryParser.SplitSqlBatches(script);
        Assert.Equal(new[] { "SELECT 1" }, batches);
    }
}
