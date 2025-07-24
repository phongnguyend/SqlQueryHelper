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
}
