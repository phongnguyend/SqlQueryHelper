namespace SqlServerQueryHelper.Tests.SqlQueryParserTests;

public class RegenerateSqlQueryTests
{
    [Theory]
    [InlineData("SELECT    *    FROM    Table1   ;")]
    [InlineData("SELECT [Col1]   FROM   [Table1]   ;")]
    [InlineData("INSERT  INTO  Table1   (Col1,   Col2)   VALUES   (1,   2)   ;")]
    [InlineData("SELECT  1   -- this is a comment\nGO")]
    [InlineData("SELECT\t/* multi\nline\ncomment */\t1")]
    [InlineData("SELECT  1\nGO\r\nSELECT   2")]
    [InlineData("SELECT   [Col1],   Col2   --comment\r\nFROM   [Table1]   /*multi\nline*/   ;")]
    [InlineData("   SELECT * FROM Table1;   ")]
    [InlineData("SELECT\t*\tFROM\tTable1;\t")]
    public void RegenerateSqlQuery_RoundTrip(string sql)
    {
        var tokens = SqlQueryParser.ParseTokens(sql);
        var regenerated = SqlQueryParser.RegenerateSqlQuery(tokens);
        Assert.Equal(sql, regenerated);
    }

    [Theory]
    [InlineData("SELECT    *    FROM    Table1   ;")]
    [InlineData("SELECT [Col1]   FROM   [Table1]   ;")]
    [InlineData("INSERT  INTO  Table1   (Col1,   Col2)   VALUES   (1,   2)   ;")]
    [InlineData("SELECT  1   -- this is a comment\nGO")]
    [InlineData("SELECT\t/* multi\nline\ncomment */\t1")]
    [InlineData("SELECT  1\nGO\r\nSELECT   2")]
    [InlineData("SELECT   [Col1],   Col2   --comment\r\nFROM   [Table1]   /*multi\nline*/   ;")]
    [InlineData("   SELECT * FROM Table1;   ")]
    [InlineData("SELECT\t*\tFROM\tTable1;\t")]
    public void RegenerateSqlQuery_FromTokenGroups_RoundTrip(string sql)
    {
        var tokens = SqlQueryParser.ParseTokens(sql);
        var tokenGroups = SqlQueryParser.GroupTokensByLine(tokens);
        var regenerated = SqlQueryParser.RegenerateSqlQuery(tokenGroups);
        Assert.Equal(sql, regenerated);
    }

    [Fact]
    public void RegenerateSqlQuery_FromTokenGroups_EmptyInput_ReturnsEmpty()
    {
        var result = SqlQueryParser.RegenerateSqlQuery(new List<List<string>>());
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RegenerateSqlQuery_FromTokenGroups_NullInput_ReturnsEmpty()
    {
        var result = SqlQueryParser.RegenerateSqlQuery((List<List<string>>)null);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RegenerateSqlQuery_FromTokenGroups_WithNullGroups_HandlesGracefully()
    {
        var tokenGroups = new List<List<string>>
        {
            new List<string> { "SELECT", " ", "1", "\n" },
            null,
            new List<string> { "FROM", " ", "Table1" }
        };
        
        var result = SqlQueryParser.RegenerateSqlQuery(tokenGroups);
        Assert.Equal("SELECT 1\nFROM Table1", result);
    }
}
