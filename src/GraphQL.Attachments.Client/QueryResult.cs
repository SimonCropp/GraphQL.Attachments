namespace GraphQL.Attachments;

public class QueryResult(Stream stream, IReadOnlyDictionary<string, Attachment> attachments, HttpContentHeaders contentHeaders, HttpHeaders headers, HttpStatusCode status) :
    IAsyncDisposable
{
    public Stream Stream { get; } = stream;
    public IReadOnlyDictionary<string, Attachment> Attachments { get; } = attachments;
    public HttpContentHeaders ContentHeaders { get; } = contentHeaders;
    public HttpHeaders Headers { get; } = headers;
    public HttpStatusCode Status { get; } = status;

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();

        foreach (var attachment in Attachments.Values)
        {
            await attachment.Stream.DisposeAsync();
        }
    }
}