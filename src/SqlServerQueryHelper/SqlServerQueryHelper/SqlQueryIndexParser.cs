namespace SqlServerQueryHelper;

public class SqlQueryIndexParser
{
    public static IndexInfo ParseIndex(string sqlQuery)
    {
        var indexInfo = new IndexInfo();

        var tokens = SqlQueryParser.ParseTokens(sqlQuery).ToArray().AsSpan();

        var index = 0;
        var indexOfCREATEKeyword = -1;
        var indexOfINDEXKeyword = -1;
        var indexOfONKeyword = -1;
        var indeOfIndexColumns = -1;
        var indexOfINCLUDEKeyword = -1;
        var indexOfWITHKeyword = -1;

        foreach (var token in tokens)
        {
            if (token.Equals("CREATE", StringComparison.OrdinalIgnoreCase))
            {
                indexOfCREATEKeyword = index;
            }

            if (token.Equals("INDEX", StringComparison.OrdinalIgnoreCase))
            {
                indexOfINDEXKeyword = index;
            }

            if (token.Equals("ON", StringComparison.OrdinalIgnoreCase))
            {
                indexOfONKeyword = index;
            }

            if (token.StartsWith("(", StringComparison.OrdinalIgnoreCase) && indexOfONKeyword > -1 && indeOfIndexColumns == -1)
            {
                indeOfIndexColumns = index;
            }

            if (token.Equals("INCLUDE", StringComparison.OrdinalIgnoreCase))
            {
                indexOfINCLUDEKeyword = index;
            }

            if (token.Equals("WITH", StringComparison.OrdinalIgnoreCase))
            {
                indexOfWITHKeyword = index;
            }

            index++;
        }

        indexInfo.Name = string.Join(string.Empty, tokens.Slice(indexOfINDEXKeyword + 1, indexOfONKeyword - indexOfINDEXKeyword - 1).ToArray()).Trim();
        indexInfo.TableName = string.Join(string.Empty, tokens.Slice(indexOfONKeyword + 1, indeOfIndexColumns - indexOfONKeyword - 1).ToArray()).Trim();

        return indexInfo;
    }
}

public class IndexInfo
{
    public string Name { get; set; }

    public string TableName { get; set; }
}
