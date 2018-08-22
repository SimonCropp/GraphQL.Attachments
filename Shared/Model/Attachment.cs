using System;
using System.IO;

public class Attachment
{
    public string Name { get; set; }
    public Func<Stream> Stream { get; set; }
}