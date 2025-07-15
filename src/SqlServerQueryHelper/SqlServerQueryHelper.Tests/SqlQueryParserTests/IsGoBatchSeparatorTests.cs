namespace SqlServerQueryHelper.Tests.SqlQueryParserTests;

public class IsGoBatchSeparatorTests
{
    [Theory]
    [InlineData("GO", true)]
    [InlineData("GO ", true)]
    [InlineData("GO;", true)]
    [InlineData("GO ;", true)]
    [InlineData("GO 100", false)] // do not support Go with a batch count
    public void IsGoBatchSeparator(string text, bool expected)
    {
        Assert.Equal(expected, SqlQueryParser.IsGoBatchSeparator(text));
    }
}
