public static class LinesScrubber
{
    public static void RemoveLineSuffix(this StringBuilder builder, string stringToMatch)
    {
        using var reader = new StringReader(builder.ToString());
        builder.Clear();
        while (reader.ReadLine() is { } line)
        {
            var split = line.Split(stringToMatch);
            builder.Append(split[0]);
            builder.Append('\n');
        }

        builder.Length -= 1;
    }
}