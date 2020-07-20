using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    /// <summary>
    /// Handles writing a <see cref="AttachmentExecutionResult"/> to a <see cref="HttpResponse"/>.
    /// </summary>
    public static class ResponseWriter
    {
        /// <summary>
        /// Writes <paramref name="result"/> to <paramref name="response"/>.
        /// </summary>
        public static Task WriteResult(HttpResponse response, AttachmentExecutionResult result, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(response), response);
            Guard.AgainstNull(nameof(result), result);
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

        static async Task WriteMultipart(
            HttpResponse response,
            ExecutionResult result,
            OutgoingAttachments attachments,
            CancellationToken cancellation)
        {
            var httpContents = new List<HttpContent>();
            try
            {
                using var multipart = new MultipartFormDataContent();
                var serializedResult = JsonConvert.SerializeObject(result);

                // Add a name for the serialized result which will enable fetch().formData() to parse the result to FormData object
                // To be reviewed by Simon
                multipart.Add(new StringContent(serializedResult), "result");

                foreach (var attachment in attachments.Inner)
                {
                    httpContents.Add(await AddAttachment(attachment, multipart, cancellation));
                }

                response.ContentLength = multipart.Headers.ContentLength;
                response.ContentType = multipart.Headers.ContentType.ToString();
                await multipart.CopyToAsync(response.Body);
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

        static Task WriteStream(ExecutionResult result, HttpResponse response, CancellationToken cancellation)
        {
            response.Headers.Add("Content-Type", "application/json");
            var serializeObject = JsonConvert.SerializeObject(result);
            return response.WriteAsync(serializeObject, cancellation);
        }
    }
}