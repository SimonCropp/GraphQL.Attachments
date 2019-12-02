using System;
using System.Collections.Generic;
using System.IO;

namespace GraphQL.Attachments
{
    public class QueryResult :
        IDisposable
    {
        public Stream ResultStream { get; set; } = null!;
        public Dictionary<string, Attachment> Attachments { get; set; } = new Dictionary<string, Attachment>();

        public void Dispose()
        {
            ResultStream?.Dispose();
            foreach (var attachment in Attachments.Values)
            {
                attachment.Stream.Dispose();
            }
        }
    }
}