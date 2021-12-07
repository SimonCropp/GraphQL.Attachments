using System.Net.Http.Headers;

namespace GraphQL.Attachments;

public class Attachment
{
    public Attachment(string name, Stream stream, HttpContentHeaders headers)
    {
        Name = name;
        Stream = stream;
        Headers = headers;
    }

    public Stream Stream { get; }
    public string Name { get; }
    public HttpContentHeaders Headers { get; }
}