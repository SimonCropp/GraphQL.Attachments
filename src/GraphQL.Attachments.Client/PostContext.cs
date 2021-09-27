using System.Net.Http;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class PostContext
    {
        MultipartFormDataContent content;

        public PostContext(MultipartFormDataContent content)
        {
            this.content = content;
        }

        public void SetHeadersAction(Action<HttpContentHeaders> headerAction)
        {
            HeadersAction = headerAction;
        }

        public Action<HttpContentHeaders>? HeadersAction { get; private set; }

        public void AddAttachment(string name, Stream value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            StreamContent file = new(value);
            content.Add(file, name, name);
        }

        public void AddAttachment(string name, byte[] value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            ByteArrayContent file = new(value);
            content.Add(file, name, name);
        }

        public void AddAttachment(string name, string value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            StringContent file = new(value);
            content.Add(file, name, name);
        }
    }
}