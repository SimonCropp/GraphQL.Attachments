using System.Net;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class QueryResult :
        IAsyncDisposable
    {
        public Stream Stream { get; }
        public IReadOnlyDictionary<string, Attachment> Attachments { get; }
        public HttpContentHeaders ContentHeaders { get; }
        public HttpStatusCode Status { get; }

        public QueryResult(Stream stream, IReadOnlyDictionary<string, Attachment> attachments, HttpContentHeaders contentHeaders, HttpStatusCode status)
        {
            Stream = stream;
            Attachments = attachments;
            ContentHeaders = contentHeaders;
            Status = status;
        }

        public async ValueTask DisposeAsync()
        {
            await Stream.DisposeAsync();

            foreach (var attachment in Attachments.Values)
            {
                await attachment.Stream.DisposeAsync();
            }
        }
    }
}