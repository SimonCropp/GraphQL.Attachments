using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public class QueryResult :
        IAsyncDisposable
    {
        public Stream ResultStream { get; set; } = null!;
        public Dictionary<string, Attachment> Attachments { get; set; } = new Dictionary<string, Attachment>();

        public QueryResult(Stream resultStream, Dictionary<string, Attachment> attachments)
        {
            ResultStream = resultStream;
            Attachments = attachments;
        }

        public QueryResult()
        {

        }

        public async ValueTask DisposeAsync()
        {
            if (ResultStream != null)
            {
                await ResultStream.DisposeAsync();
            }

            foreach (var attachment in Attachments.Values)
            {
                await attachment.Stream.DisposeAsync();
            }
        }
    }
}