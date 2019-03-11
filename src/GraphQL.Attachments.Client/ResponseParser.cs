using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public static class ResponseParser
    {
        public static async Task ProcessResponse(this HttpResponseMessage response, Action<Stream> resultAction, Action<Attachment> attachmentAction, CancellationToken cancellation = default)
        {
            if (!response.IsMultipart())
            {
                resultAction(await response.Content.ReadAsStreamAsync());
                return;
            }

            var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
            await ProcessBody(multipart, resultAction);

            foreach (var content in multipart.Contents.Skip(1))
            {
                if (attachmentAction == null)
                {
                    throw new Exception("Found an attachment but handler had no AttachmentAction.");
                }

                var name = content.Headers.ContentDisposition.Name;
                var stream = await content.ReadAsStreamAsync();

                var attachment = new Attachment
                {
                    Name = name,
                    Stream = stream,
                    Headers = content.Headers,
                };
                attachmentAction(attachment);
            }
        }

        static async Task ProcessBody(MultipartStreamProvider multipart, Action<Stream> resultAction)
        {
            var first = multipart.Contents.FirstOrDefault();
            if (first == null)
            {
                throw new Exception("Expected the multipart response have at least one part which contains the graphql response data.");
            }

            var name = first.Headers.ContentDisposition.Name;
            if (name != null)
            {
                throw new Exception("Expected the first part in the multipart response to be un-named.");
            }

            var stream = await first.ReadAsStreamAsync();
            resultAction(stream);
        }
    }
}