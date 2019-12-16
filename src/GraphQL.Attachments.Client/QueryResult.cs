using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public class QueryResult :
        IAsyncDisposable
    {
        public Stream Stream { get; }
        public IReadOnlyDictionary<string, Attachment> Attachments { get; }
        public HttpContentHeaders ContentHeaders { get; }

        public QueryResult(Stream stream, IReadOnlyDictionary<string, Attachment> attachments, HttpContentHeaders contentHeaders)
        {
            Guard.AgainstNull(nameof(stream), stream);
            Guard.AgainstNull(nameof(attachments), attachments);
            Stream = stream;
            Attachments = attachments;
            ContentHeaders= contentHeaders;
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