namespace SqlServerQueryHelper.Tests.SqlQueryGeneratorTests;

public class CountDuplicatedRecordsTests
{
    [Theory]
    [InlineData("[MyTable]", new string[] { "Col1" }, "SELECT [Col1], COUNT(*) AS COUNT\nFROM [MyTable]\nGROUP BY [Col1]\nHAVING COUNT(*) > 1\n")]
    [InlineData("[MyTable]", new string[] { "Col1", "Col2" }, "SELECT [Col1], [Col2], COUNT(*) AS COUNT\nFROM [MyTable]\nGROUP BY [Col1], [Col2]\nHAVING COUNT(*) > 1\n")]
    [InlineData("[TestTable]", new string[] { "A", "B", "C" }, "SELECT [A], [B], [C], COUNT(*) AS COUNT\nFROM [TestTable]\nGROUP BY [A], [B], [C]\nHAVING COUNT(*) > 1\n")]
    [InlineData("[ArchivedEmails]", new string[] { "Subject", "MailFrom", "MailTo" }, "SELECT [Subject], [MailFrom], [MailTo], COUNT(*) AS COUNT\nFROM [ArchivedEmails]\nGROUP BY [Subject], [MailFrom], [MailTo]\nHAVING COUNT(*) > 1\n")]
    public void CountDuplicatedRecords_GeneratesCorrectSql(string tableName, string[] duplicatedColumns, string expectedSql)
    {
        // Act
        var sql = SqlQueryGenerator.CountDuplicatedRecords(tableName, duplicatedColumns);

        // Normalize whitespace for comparison
        var normalizedSql = sql.Replace("\r\n", "\n").Trim();
        var normalizedExpected = expectedSql.Replace("\r\n", "\n").Trim();

        // Assert
        Assert.Equal(normalizedExpected, normalizedSql);
    }
}
