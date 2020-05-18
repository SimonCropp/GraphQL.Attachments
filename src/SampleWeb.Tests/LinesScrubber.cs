using System.IO;
using System.Text;

public static class LinesScrubber
{
    public static void RemoveLineSuffix(this StringBuilder builder, string stringToMatch)
    {
        using var reader = new StringReader(builder.ToString());
        builder.Clear();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var split = line.Split(stringToMatch);
            builder.AppendLine(split[0]);
        }

        builder.Length -= 1;
    }
}