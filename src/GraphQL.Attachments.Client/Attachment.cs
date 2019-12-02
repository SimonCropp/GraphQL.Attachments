using System.IO;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class Attachment
    {
        public Attachment(string name, Stream stream, HttpContentHeaders headers)
        {
            Guard.AgainstNull(nameof(name), name);
            Guard.AgainstNull(nameof(stream), stream);
            Guard.AgainstNull(nameof(headers), headers);
            Name = name;
            Stream = stream;
            Headers = headers;
        }

        public Stream Stream { get; }
        public string Name { get; }
        public HttpContentHeaders Headers { get; }
    }
}