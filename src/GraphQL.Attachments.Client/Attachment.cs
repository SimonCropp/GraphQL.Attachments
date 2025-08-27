namespace GraphQL.Attachments;

public class Attachment(string name, Stream stream, HttpContentHeaders headers)
{
    public Stream Stream { get; } = stream;
    public string Name { get; } = name;
    public HttpContentHeaders Headers { get; } = headers;
}