namespace GraphQL.Attachments;

public partial class HttpReaderWriter
{
    /// <summary>
    /// Writes <paramref name="result"/> to <paramref name="response"/>.
    /// </summary>
    public Task WriteResult(HttpResponse response, AttachmentExecutionResult result, Cancel cancel = default)
    {
        var executionResult = result.ExecutionResult;
        var attachments = (OutgoingAttachments) result.Attachments;
        if (response.StatusCode == (int) HttpStatusCode.OK &&
            executionResult.Errors?.Count > 0)
        {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            return WriteStream(executionResult, response, cancel);
        }

        if (attachments.HasPendingAttachments)
        {
            return WriteMultipart(response, executionResult, attachments, cancel);
        }

        return WriteStream(executionResult, response, cancel);
    }

    async Task WriteMultipart(
        HttpResponse response,
        ExecutionResult result,
        OutgoingAttachments attachments,
        Cancel cancel)
    {
        var httpContents = new List<HttpContent>();
        try
        {
            var contentStream = new MemoryStream();
            await serializer.WriteAsync(contentStream, result, cancel);
            contentStream.Position = 0;
            using var multipart = new MultipartFormDataContent
            {
                {new StreamContent(contentStream), "result"}
            };

            foreach (var attachment in attachments.Inner)
            {
                httpContents.Add(await AddAttachment(attachment, multipart, cancel));
            }

            response.ContentLength = multipart.Headers.ContentLength;
            response.ContentType = multipart.Headers.ContentType?.ToString()!;
            await multipart.CopyToAsync(response.Body, cancel);
        }
        finally
        {
            foreach (var httpContent in httpContents)
            {
                httpContent.Dispose();
            }

            foreach (var cleanup in attachments.Inner.Select(_ => _.Value.Cleanup))
            {
                cleanup?.Invoke();
            }
        }
    }

    static async Task<HttpContent> AddAttachment(
        KeyValuePair<string, Outgoing> attachment,
        MultipartFormDataContent multipart,
        Cancel cancel)
    {
        var outgoing = attachment.Value;
        var content = await outgoing.ContentBuilder(cancel);
        if (outgoing.Headers != null)
        {
            foreach (var (key, value) in outgoing.Headers)
            {
                content.Headers.Add(key, value);
            }
        }

        multipart.Add(content, attachment.Key, Uri.EscapeDataString(attachment.Key));
        return content;
    }

    Task WriteStream(
        ExecutionResult result,
        HttpResponse response,
        Cancel cancel)
    {
        var headers = response.Headers;
        if (result.Query.Span.StartsWith("mutation"))
        {
            headers["Cache-Control"] = "no-store, max-age=0";
        }
        else
        {
            headers["Cache-Control"] = "no-cache";
        }
        headers["Content-Type"] = "application/json; charset=utf-8";
        headers["X-Content-Type-Options"] = "nosniff";
        return serializer.WriteAsync(response.Body, result, cancel);
    }
}