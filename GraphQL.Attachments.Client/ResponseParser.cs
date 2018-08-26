using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Attachments;

static class ResponseParser
{
    public static async Task ProcessResponse(HttpResponseMessage response, QueryResponseHandler handler, CancellationToken cancellation = default)
    {
        if (response.IsMultipart())
        {
            var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
            var resultProcessed = false;
            foreach (var content in multipart.Contents)
            {
                var name = content.Headers.ContentDisposition.Name;
                var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
                if (name == null)
                {
                    if (resultProcessed)
                    {
                        throw new Exception("Expected the multipart response to contain a single un-named part which contains the graphql response data.");
                    }

                    resultProcessed = true;
                    handler.ResultAction(stream);
                }
                else
                {
                    if (handler.AttachmentAction == null)
                    {
                        throw new Exception("Found an attachment but handler had no AttachmentAction.");
                    }
                    var attachment = new Attachment
                    {
                        Name = name,
                        Stream = stream,
                        Headers = content.Headers,
                    };
                    handler.AttachmentAction(attachment);
                }
            }

            if (!resultProcessed)
            {
                throw new Exception("Expected the multipart response to contain a single un-named part which contains the graphql response data.");
            }
        }
        else
        {
            handler.ResultAction(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }
    }
}