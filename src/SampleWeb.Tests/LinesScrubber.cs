public static class LinesScrubber
{
    public static void RemoveLineSuffix(this StringBuilder builder, string stringToMatch)
    {
        using StringReader reader = new(builder.ToString());
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