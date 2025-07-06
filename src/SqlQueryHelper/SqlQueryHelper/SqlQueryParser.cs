namespace SqlQueryHelper;

public class SqlQueryParser
{
    public static IndexInfo ParseIndex(string sqlQuery)
    {
        var indexInfo = new IndexInfo();

        var tokens = sqlQuery
            .Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        var index = 0;
        var indexOfCREATEKeyword = -1;
        var indexOfINDEXKeyword = -1;
        var indexOfONKeyword = -1;
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

        indexInfo.Name = tokens[indexOfINDEXKeyword + 1];

        indexInfo.TableName = tokens[indexOfONKeyword + 1];

        return indexInfo;
    }
}

public class IndexInfo
{
    public string Name { get; set; }

    public string TableName { get; set; }
}
