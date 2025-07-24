namespace SqlServerQueryHelper.Tests.SqlQueryParserTests;

public class ParseTokensTests
{
    [Fact]
    public void SimpleSql_Tokens()
    {
        var sql = "SELECT * FROM Table1;";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Equal(new List<string> { "SELECT"," ", "*", " ", "FROM", " ", "Table1", ";" }, tokens);
        Assert.Equal(8, tokens.Count);
    }

    [Fact]
    public void BracketedIdentifiers_Tokens()
    {
        var sql = "SELECT [Col1] FROM [Table1];";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("[Col1]", tokens);
        Assert.Contains("[Table1]", tokens);
        Assert.Equal(8, tokens.Count);
    }

    [Fact]
    public void ParenthesesAndPunctuation_Tokens()
    {
        var sql = "INSERT INTO Table1 (Col1,Col2) VALUES (1,2);";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("(Col1,Col2)", tokens);
        Assert.Contains("(1,2)", tokens);
        Assert.Contains(";", tokens);
        Assert.Equal(12, tokens.Count);
    }

    [Fact]
    public void SingleLineComment_Token()
    {
        var sql = "SELECT 1 -- this is a comment\nGO";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("-- this is a comment", tokens[4]);
        Assert.Equal(7, tokens.Count);
    }

    [Fact]
    public void MultiLineComment_Token()
    {
        var sql = "SELECT /* multi\nline\ncomment */ 1";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("/* multi\nline\ncomment */", tokens);
        Assert.Equal(5, tokens.Count);
    }

    [Fact]
    public void NewLines_AreSpecialTokens()
    {
        var sql = "SELECT 1\nGO\r\nSELECT 2";
        var tokens = SqlQueryParser.ParseTokens(sql);
        Assert.Contains("\n", tokens);
        Assert.Contains("\r\n", tokens);
        Assert.Equal(9, tokens.Count);
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
        Assert.Equal(15, tokens.Count);
    }

    [Fact]
    public void GroupTokensByLine_SingleLine_GroupsCorrectly()
    {
        var sql = "SELECT * FROM Table1;";
        var tokens = SqlQueryParser.ParseTokens(sql);
        var groupedTokens = SqlQueryParser.GroupTokensByLine(tokens);
        
        Assert.Single(groupedTokens);
        Assert.Equal(new List<string> { "SELECT", " ", "*", " ", "FROM", " ", "Table1", ";" }, groupedTokens[0]);
    }

    [Fact]
    public void GroupTokensByLine_MultipleLines_GroupsCorrectly()
    {
        var sql = "SELECT 1\nGO\r\nSELECT 2";
        var tokens = SqlQueryParser.ParseTokens(sql);
        var groupedTokens = SqlQueryParser.GroupTokensByLine(tokens);
        
        Assert.Equal(3, groupedTokens.Count);
        Assert.Equal(new List<string> { "SELECT", " ", "1", "\n" }, groupedTokens[0]);
        Assert.Equal(new List<string> { "GO", "\r\n" }, groupedTokens[1]);
        Assert.Equal(new List<string> { "SELECT", " ", "2" }, groupedTokens[2]);
    }

    [Fact]
    public void GroupTokensByLine_EmptyLines_PreservesStructure()
    {
        var sql = "SELECT 1\n\nSELECT 2";
        var tokens = SqlQueryParser.ParseTokens(sql);
        var groupedTokens = SqlQueryParser.GroupTokensByLine(tokens);
        
        Assert.Equal(3, groupedTokens.Count);
        Assert.Equal(new List<string> { "SELECT", " ", "1", "\n" }, groupedTokens[0]);
        Assert.Equal(new List<string> { "\n" }, groupedTokens[1]); // Empty line with newline
        Assert.Equal(new List<string> { "SELECT", " ", "2" }, groupedTokens[2]);
    }

    [Fact]
    public void GroupTokensByLine_WithComments_GroupsCorrectly()
    {
        var sql = "SELECT 1 --comment\r\nFROM Table1";
        var tokens = SqlQueryParser.ParseTokens(sql);
        var groupedTokens = SqlQueryParser.GroupTokensByLine(tokens);
        
        Assert.Equal(2, groupedTokens.Count);
        Assert.Equal(new List<string> { "SELECT", " ", "1", " ", "--comment", "\r\n" }, groupedTokens[0]);
        Assert.Equal(new List<string> { "FROM", " ", "Table1" }, groupedTokens[1]);
    }

    [Fact]
    public void GroupTokensByLine_EmptyTokenList_ReturnsEmpty()
    {
        var tokens = new List<string>();
        var groupedTokens = SqlQueryParser.GroupTokensByLine(tokens);
        
        Assert.Empty(groupedTokens);
    }

    [Fact]
    public void GroupTokensByLine_NullTokenList_ReturnsEmpty()
    {
        var groupedTokens = SqlQueryParser.GroupTokensByLine(null);
        
        Assert.Empty(groupedTokens);
    }
}
