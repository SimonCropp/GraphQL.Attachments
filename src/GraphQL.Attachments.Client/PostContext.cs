namespace GraphQL.Attachments;

public class PostContext(MultipartFormDataContent content)
{
    public void SetHeadersAction(Action<HttpContentHeaders> headerAction) =>
        HeadersAction = headerAction;

    public Action<HttpContentHeaders>? HeadersAction { get; private set; }

    public void AddAttachment(string name, Stream value)
    {
        Guard.AgainstNullWhiteSpace(name);
        var file = new StreamContent(value);
        content.Add(file, name, name);
    }

    public void AddAttachment(string name, byte[] value)
    {
        Guard.AgainstNullWhiteSpace(name);
        var file = new ByteArrayContent(value);
        content.Add(file, name, name);
    }

    public void AddAttachment(string name, string value)
    {
        Guard.AgainstNullWhiteSpace( name);
        var file = new StringContent(value);
        content.Add(file, name, name);
    }
}