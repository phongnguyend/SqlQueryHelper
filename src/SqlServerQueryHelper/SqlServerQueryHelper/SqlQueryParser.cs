using System.Text;

namespace SqlServerQueryHelper;

public class SqlQueryParser
{
    public static List<string> ParseTokens(string sqlQuery)
    {
        var tokens = new List<string>();
        var currentToken = new StringBuilder();

        var index = 0;
        while (index < sqlQuery.Length)
        {
            var c = sqlQuery[index];

            // Handle single-line comment as a special token
            if (c == '-' && index + 1 < sqlQuery.Length && sqlQuery[index + 1] == '-')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
                string comment = ParseSingleLineComment(sqlQuery, ref index);
                tokens.Add(comment);
                continue;
            }

            // Handle multi-line comment as a special token
            if (c == '/' && index + 1 < sqlQuery.Length && sqlQuery[index + 1] == '*')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
                string comment = ParseMultiLinesComment(sqlQuery, ref index);
                tokens.Add(comment);
                continue;
            }

            // Preserve newlines as special tokens
            if (c == '\r' || c == '\n')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
                if (c == '\r' && index + 1 < sqlQuery.Length && sqlQuery[index + 1] == '\n')
                {
                    tokens.Add("\r\n");
                    index += 2;
                    continue;
                }
                else
                {
                    tokens.Add(c.ToString());
                    index++;
                    continue;
                }
            }

            if (char.IsWhiteSpace(c))
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }

                if (c != '\r' && c != '\n')
                {
                    tokens.Add(c.ToString());
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
            else if (c == '.' || c == ',' || c == '=' || c == ';')
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

        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken.ToString());
            currentToken.Clear();
        }

        return tokens;
    }

    private static string ParseSingleLineComment(string sqlQuery, ref int index)
    {
        int start = index;
        index += 2;
        while (index < sqlQuery.Length && sqlQuery[index] != '\n' && sqlQuery[index] != '\r')
        {
            index++;
        }
        // Do not increment index here, let ParseTokens handle it
        return sqlQuery.Substring(start, index - start);
    }

    private static string ParseMultiLinesComment(string sqlQuery, ref int index)
    {
        int start = index;
        index += 2;
        while (index < sqlQuery.Length - 1)
        {
            if (sqlQuery[index] == '*' && sqlQuery[index + 1] == '/')
            {
                index += 2;
                break;
            }
            index++;
        }
        return sqlQuery.Substring(start, index - start);
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

    public static IEnumerable<string> SplitSqlBatches(string script)
    {
        var tokens = ParseTokens(script);
        var lines = GroupTokensByLine(tokens);

        var batch = new List<string>();

        foreach (var line in lines)
        {
            if (IsGoBatchSeparator(line))
            {
                if (batch.Count > 0)
                {
                    yield return string.Join(string.Empty, batch).Trim();
                    batch.Clear();
                }
            }
            else
            {
                batch.Add(RegenerateSqlQuery(line));
            }
        }

        if (batch.Count > 0)
        {
            yield return string.Join(string.Empty, batch).Trim();
        }
    }

    public static bool IsGoBatchSeparator(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return false;
        }

        tokens = tokens.Where(t => !string.IsNullOrWhiteSpace(t)
                                    && !t.StartsWith("--", StringComparison.OrdinalIgnoreCase) // Exclude single-line comments
                                    && !t.StartsWith("/*", StringComparison.OrdinalIgnoreCase) // Exclude multi-line comments
        ).ToList();

        if (tokens.Count == 1 && string.Equals(tokens[0], "GO", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (tokens.Count == 2 && string.Equals(tokens[0], "GO", StringComparison.OrdinalIgnoreCase) && tokens[1] == ";")
        {
            return true;
        }

        return false;
    }

    public static string RegenerateSqlQuery(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(string.Empty, tokens);
    }

    public static string RegenerateSqlQuery(List<List<string>> tokenGroups)
    {
        if (tokenGroups == null || tokenGroups.Count == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder();

        foreach (var tokenGroup in tokenGroups)
        {
            if (tokenGroup == null)
            {
                continue;
            }

            result.Append(RegenerateSqlQuery(tokenGroup));
        }

        return result.ToString();
    }

    /// <summary>
    /// Groups tokens by line based on newline delimiters (\n and \r\n).
    /// Each group represents tokens that belong to the same line, including the newline delimiter.
    /// </summary>
    /// <param name="tokens">The list of tokens to group</param>
    /// <returns>A list of token groups, where each group represents a line</returns>
    public static List<List<string>> GroupTokensByLine(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return new List<List<string>>();
        }

        var result = new List<List<string>>();
        var currentLine = new List<string>();

        foreach (var token in tokens)
        {
            // Add token to current line (including newline delimiters)
            currentLine.Add(token);

            // Check if token is a newline delimiter
            if (token == "\n" || token == "\r\n")
            {
                // Add current line to result (including the newline delimiter)
                result.Add(new List<string>(currentLine));
                currentLine.Clear();
            }
        }

        // Add the last line if it has any tokens
        if (currentLine.Count > 0)
        {
            result.Add(currentLine);
        }

        return result;
    }
}
