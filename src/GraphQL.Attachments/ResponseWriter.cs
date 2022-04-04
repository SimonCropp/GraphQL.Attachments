using Microsoft.AspNetCore.Http;
using System.Net;

namespace GraphQL.Attachments;

/// <summary>
/// Handles writing a <see cref="AttachmentExecutionResult"/> to a <see cref="HttpResponse"/>.
/// </summary>
public static class ResponseWriter
{
    /// <summary>
    /// Writes <paramref name="result"/> to <paramref name="response"/>.
    /// </summary>
    public static Task WriteResult(IGraphQLSerializer writer, HttpResponse response, AttachmentExecutionResult result, CancellationToken cancellation = default)
    {
        var executionResult = result.ExecutionResult;
        var attachments = (OutgoingAttachments) result.Attachments;
        if (response.StatusCode == (int) HttpStatusCode.OK && executionResult.Errors?.Count > 0)
        {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            return WriteStream(writer, executionResult, response, cancellation);
        }

        if (attachments.HasPendingAttachments)
        {
            return WriteMultipart(writer, response, executionResult, attachments, cancellation);
        }
        return WriteStream(writer, executionResult, response, cancellation);
    }

    static async Task WriteMultipart(
        IGraphQLSerializer writer,
        HttpResponse response,
        ExecutionResult result,
        OutgoingAttachments attachments,
        CancellationToken cancellation)
    {
        List<HttpContent> httpContents = new();
        try
        {
            var contentStream = new MemoryStream();
            await writer.WriteAsync(contentStream,result, cancellation);
            contentStream.Position = 0;
            using var multipart = new MultipartFormDataContent
            {
                {new StreamContent(contentStream), "result"}
            };

            foreach (var attachment in attachments.Inner)
            {
                httpContents.Add(await AddAttachment(attachment, multipart, cancellation));
            }

            response.ContentLength = multipart.Headers.ContentLength;
            response.ContentType = multipart.Headers.ContentType?.ToString();
            await multipart.CopyToAsync(response.Body, cancellation);
        }
        finally
        {
            foreach (var httpContent in httpContents)
            {
                httpContent.Dispose();
            }

            foreach (var cleanup in attachments.Inner.Select(x => x.Value.Cleanup))
            {
                cleanup?.Invoke();
            }
        }
    }

    static async Task<HttpContent> AddAttachment(
        KeyValuePair<string, Outgoing> attachment,
        MultipartFormDataContent multipart,
        CancellationToken cancellation)
    {
        var outgoing = attachment.Value;
        var content = await outgoing.ContentBuilder(cancellation);
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

    static async Task WriteStream(
        IGraphQLSerializer writer,
        ExecutionResult result,
        HttpResponse response,
        CancellationToken cancellation)
    {
        response.Headers.Add("Content-Type", "application/json");
        await writer.WriteAsync(response.Body, result, cancellation);
    }
}