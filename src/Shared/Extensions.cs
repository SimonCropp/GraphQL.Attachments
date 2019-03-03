using System.IO;

public static class Extensions
{
    public static string ConvertToString(this Stream stream)
    {
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}