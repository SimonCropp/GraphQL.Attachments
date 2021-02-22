using System.IO;

public static class Extensions
{
    public static string ConvertToString(this Stream stream)
    {
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}