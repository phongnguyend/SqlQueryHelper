namespace SqlServerQueryHelper.Tests.SqlQueryParserTests;

public class ParseTokensTests
{
    [Fact]
    public void SimpleSql_Tokens()
    {
        var sql = "SELECT * FROM Table1;";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Equal(new List<string> { "SELECT", "*", "FROM", "Table1", ";" }, tokens);
        Assert.Equal(5, tokens.Count);
    }

    [Fact]
    public void BracketedIdentifiers_Tokens()
    {
        var sql = "SELECT [Col1] FROM [Table1];";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("[Col1]", tokens);
        Assert.Contains("[Table1]", tokens);
        Assert.Equal(5, tokens.Count);
    }

    [Fact]
    public void ParenthesesAndPunctuation_Tokens()
    {
        var sql = "INSERT INTO Table1 (Col1,Col2) VALUES (1,2);";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("(Col1,Col2)", tokens);
        Assert.Contains("(1,2)", tokens);
        Assert.Contains(";", tokens);
        Assert.Equal(7, tokens.Count);
    }

    [Fact]
    public void SingleLineComment_Token()
    {
        var sql = "SELECT 1 -- this is a comment\nGO";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("-- this is a comment", tokens[2]);
        Assert.Equal(5, tokens.Count);
    }

    [Fact]
    public void MultiLineComment_Token()
    {
        var sql = "SELECT /* multi\nline\ncomment */ 1";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("/* multi\nline\ncomment */", tokens);
        Assert.Equal(3, tokens.Count);
    }

    [Fact]
    public void NewLines_AreSpecialTokens()
    {
        var sql = "SELECT 1\nGO\r\nSELECT 2";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("\n", tokens);
        Assert.Contains("\r\n", tokens);
        Assert.Equal(7, tokens.Count);
    }

    [Fact]
    public void MixedContent_Tokens()
    {
        var sql = "SELECT [Col1], Col2 --comment\r\nFROM [Table1] /*multi\nline*/;";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("[Col1]", tokens);
        Assert.Contains(",", tokens);
        Assert.Contains("Col2", tokens);
        Assert.Contains("--comment", tokens);
        Assert.Contains("\r\n", tokens);
        Assert.Contains("[Table1]", tokens);
        Assert.Contains("/*multi\nline*/", tokens);
        Assert.Contains(";", tokens);
        Assert.Equal(10, tokens.Count);
    }
}
