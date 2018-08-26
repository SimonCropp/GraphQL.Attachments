using System.IO;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class IncomingAttachment
    {
        public Stream Stream { get; set; }
        public string Name { get; set; }
        public HttpContentHeaders Headers { get; set; }
    }
}