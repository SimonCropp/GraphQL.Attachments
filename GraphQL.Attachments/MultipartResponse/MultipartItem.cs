using System.IO;

public class MultipartItem
{
    public string ContentType { get; set; }

    public string FileName { get; set; }

    public Stream Stream { get; set; }
}