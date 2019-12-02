using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class PostContext
    {
        MultipartFormDataContent content;

        public PostContext(MultipartFormDataContent content)
        {
            Guard.AgainstNull(nameof(content), content);
            this.content = content;
        }

        public void SetHeadersAction(Action<HttpContentHeaders> headerAction)
        {
            Guard.AgainstNull(nameof(headerAction), headerAction);
            HeadersAction = headerAction;
        }

        public Action<HttpContentHeaders>? HeadersAction { get; private set; }

        public void AddAttachment(string name, Stream value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            var file = new StreamContent(value);
            content.Add(file, name, name);
        }

        public void AddAttachment(string name, byte[] value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            var file = new ByteArrayContent(value);
            content.Add(file, name, name);
        }

        public void AddAttachment(string name, string value)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            var file = new StringContent(value);
            content.Add(file, name, name);
        }
    }
}