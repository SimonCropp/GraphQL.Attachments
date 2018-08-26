using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Attachments;

static class ResponseParser
{
    public static async Task ProcessResponse(HttpResponseMessage response, Action<Stream> resultAction, Action<IncomingAttachment> attachmentAction, CancellationToken cancellation = default)
    {
        if (response.IsMultipart())
        {
            var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
            var resultProcessed = false;
            foreach (var multipartContent in multipart.Contents)
            {
                var name = multipartContent.Headers.ContentDisposition.Name;
                var stream = await multipartContent.ReadAsStreamAsync().ConfigureAwait(false);
                if (name == null)
                {
                    if (resultProcessed)
                    {
                        throw new Exception("Expected the multipart response to contain a single un-named part which contains the graphql response data.");
                    }

                    resultProcessed = true;
                    resultAction(stream);
                }
                else
                {
                    var attachment = new IncomingAttachment
                    {
                        Name = name,
                        Stream = stream,
                        Headers = multipartContent.Headers,
                    };
                    attachmentAction(attachment);
                }
            }

            if (!resultProcessed)
            {
                throw new Exception("Expected the multipart response to contain a single un-named part which contains the graphql response data.");
            }
        }
        else
        {
            resultAction(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }
    }

    public static async Task<QueryResult> EvaluateResponse(HttpResponseMessage response, CancellationToken cancellation = default)
    {
        var queryResult = new QueryResult(response);
        await ProcessResponse(
                response: response,
                resultAction: stream => queryResult.ResultStream = stream,
                attachmentAction: attachment => queryResult.Attachments.Add(attachment.Name, attachment), cancellation)
            .ConfigureAwait(false);
        return queryResult;
    }
}