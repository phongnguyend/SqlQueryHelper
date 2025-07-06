using System.Text;

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

    public static IndexInfo ParseIndexV2(string sqlQuery)
    {
        var indexInfo = new IndexInfo();

        var tokens = ParseTokens(sqlQuery).ToArray().AsSpan();

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

        indexInfo.Name = string.Join(string.Empty, tokens.Slice(indexOfINDEXKeyword + 1, indexOfONKeyword - indexOfINDEXKeyword - 1).ToArray());
        indexInfo.TableName = string.Join(string.Empty, tokens.Slice(indexOfONKeyword + 1, indeOfIndexColumns - indexOfONKeyword - 1).ToArray());

        return indexInfo;
    }

    private static List<string> ParseTokens(string sqlQuery)
    {
        var tokens = new List<string>();
        var currentToken = new StringBuilder();

        var index = 0;
        while (index < sqlQuery.Length)
        {
            var c = sqlQuery[index];

            if (char.IsWhiteSpace(c))
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
            }
            else if (c == '[')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }

                var quotedString = ParseQuotedString(sqlQuery, index, ref index);
                tokens.Add(quotedString);
            }
            else if (c == '(')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }

                var groupedString = ParseGroupedString(sqlQuery, index, ref index);
                tokens.Add(groupedString);
            }
            else if (c == '.' || c == ',' || c == '=')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }

                tokens.Add(c.ToString());
            }
            else
            {
                currentToken.Append(c);
            }

            index++;
        }

        return tokens;
    }

    private static string ParseQuotedString(string sqlQuery, int start, ref int index)
    {
        index++;

        while (index < sqlQuery.Length)
        {
            var c = sqlQuery[index];

            if (c == ']')
            {
                if (index + 1 < sqlQuery.Length && sqlQuery[index + 1] == ']')
                {
                    // Handle escaped closing bracket
                    index++;
                }
                else
                {
                    // Found the end of the quoted string
                    var rs = sqlQuery.Substring(start, index - start + 1);
                    return rs;
                }
            }

            index++;
        }

        throw new Exception("Invalid Syntax.");
    }

    private static string ParseGroupedString(string sqlQuery, int start, ref int index)
    {
        var count = 1;

        index++;

        while (index < sqlQuery.Length)
        {
            var c = sqlQuery[index];

            if (c == '(')
            {
                count++;
            }
            else if (c == ')')
            {
                count--;

                if (count == 0)
                {
                    var rs = sqlQuery.Substring(start, index - start + 1);
                    return rs;
                }
            }

            index++;
        }

        throw new Exception("Invalid Syntax.");
    }
}

public class IndexInfo
{
    public string Name { get; set; }

    public string TableName { get; set; }
}
