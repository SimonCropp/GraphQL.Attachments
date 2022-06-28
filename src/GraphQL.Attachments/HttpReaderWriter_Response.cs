﻿using Microsoft.AspNetCore.Http;
using System.Net;

namespace GraphQL.Attachments;

public partial class HttpReaderWriter
{
    /// <summary>
    /// Writes <paramref name="result"/> to <paramref name="response"/>.
    /// </summary>
    public Task WriteResult(HttpResponse response, AttachmentExecutionResult result, CancellationToken cancellation = default)
    {
        var executionResult = result.ExecutionResult;
        var attachments = (OutgoingAttachments) result.Attachments;
        if (response.StatusCode == (int) HttpStatusCode.OK && executionResult.Errors?.Count > 0)
        {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            return WriteStream(executionResult, response, cancellation);
        }

        if (attachments.HasPendingAttachments)
        {
            return WriteMultipart(response, executionResult, attachments, cancellation);
        }

        return WriteStream(executionResult, response, cancellation);
    }

    async Task WriteMultipart(
        HttpResponse response,
        ExecutionResult result,
        OutgoingAttachments attachments,
        CancellationToken cancellation)
    {
        List<HttpContent> httpContents = new();
        try
        {
            var contentStream = new MemoryStream();
            await serializer.WriteAsync(contentStream, result, cancellation);
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

    Task WriteStream(
        ExecutionResult result,
        HttpResponse response,
        CancellationToken cancellation)
    {
        response.Headers.Add("Content-Type", "application/json");
        return serializer.WriteAsync(response.Body, result, cancellation);
    }
}