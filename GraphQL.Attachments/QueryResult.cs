using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace GraphQL.Attachments
{
    public class QueryResult:IDisposable
    {
        HttpResponseMessage response;

        public QueryResult(HttpResponseMessage response)
        {
            this.response = response;
        }

        public Stream ResultStream { get; set; }
        public Dictionary<string, Stream> Attachments { get; set; } = new Dictionary<string, Stream>();

        public void Dispose()
        {
            response.Dispose();
            ResultStream?.Dispose();
            foreach (var attachment in Attachments.Values)
            {
                attachment.Dispose();
            }
        }
    }
}