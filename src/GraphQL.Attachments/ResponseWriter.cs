using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class ResponseWriter
{
    public static async Task WriteResult(AttachmentContext attachmentContext, HttpResponse httpResponse, ExecutionResult result)
    {
        var responseBody = httpResponse.Body;
        if (result.Errors?.Count > 0)
        {
            httpResponse.StatusCode = (int) HttpStatusCode.BadRequest;
            await WriteResult(responseBody, result);
            return;
        }

        var outgoingAttachments = (OutgoingAttachments) attachmentContext.Outgoing;
        if (outgoingAttachments.HasPendingAttachments)
        {
            using (var multipartContent = new MultipartFormDataContent())
            {
                foreach (var outgoingAttachment in outgoingAttachments.Inner)
                {
                    var outgoing = outgoingAttachment.Value;
                    var httpContent = await BuildContent(outgoing);
                    if (outgoing.Headers != null)
                    {
                        foreach (var header in outgoing.Headers)
                        {
                            httpContent.Headers.Add(header.Key, header.Value);
                        }
                    }

                    multipartContent.Add(httpContent, outgoingAttachment.Key, outgoingAttachment.Key);
                }

                var serializedResult = JsonConvert.SerializeObject(result);
                multipartContent.Add(new StringContent(serializedResult));
                httpResponse.ContentLength = multipartContent.Headers.ContentLength;
                httpResponse.ContentType = multipartContent.Headers.ContentType.ToString();
                await multipartContent.CopyToAsync(responseBody);
            }

            return;
        }

        await WriteResult(responseBody, result);
    }

    static async Task<HttpContent> BuildContent(Outgoing outgoing)
    {
        if (outgoing.AsyncStreamFactory != null)
        {
            var value = await outgoing.AsyncStreamFactory();
            return new StreamContent(value);
        }

        if (outgoing.StreamFactory != null)
        {
            return new StreamContent(outgoing.StreamFactory());
        }

        if (outgoing.StreamInstance != null)
        {
            return new StreamContent(outgoing.StreamInstance);
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var value = await outgoing.AsyncBytesFactory();
            return new ByteArrayContent(value);
        }

        if (outgoing.BytesFactory != null)
        {
            return new ByteArrayContent(outgoing.BytesFactory());
        }

        if (outgoing.BytesInstance != null)
        {
            return new ByteArrayContent(outgoing.BytesInstance);
        }

        if (outgoing.AsyncStringFactory != null)
        {
            var value = await outgoing.AsyncStringFactory();
            return new StringContent(value);
        }

        if (outgoing.StringFactory != null)
        {
            return new StringContent(outgoing.StringFactory());
        }

        if (outgoing.StringInstance != null)
        {
            return new StringContent(outgoing.StringInstance);
        }

        throw new Exception("No matching way to handle outgoing.");
    }

    static async Task WriteResult(Stream stream, ExecutionResult result)
    {
        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
        {
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}