using System.IO;
using System.Text;

public static class LinesScrubber
{
    public static string RemoveLineSuffix(this string input, string stringToMatch)
    {
        using var reader = new StringReader(input);
        var builder = new StringBuilder();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var split = line.Split(stringToMatch);
            builder.AppendLine(split[0]);
        }

        return builder.ToString();
    }
}