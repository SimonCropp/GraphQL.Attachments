using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
        public static async Task WriteResult(HttpResponse response, AttachmentExecutionResult result)
        {
            Guard.AgainstNull(nameof(response), response);
            Guard.AgainstNull(nameof(result), result);
            var executionResult = result.ExecutionResult;
            var outgoingAttachments = (OutgoingAttachments) result.Attachments;
            var body = response.Body;
            if (executionResult.Errors?.Count > 0)
            {
                response.StatusCode = (int) HttpStatusCode.BadRequest;
                await WriteResult(body, executionResult);
                return;
            }

            if (!outgoingAttachments.HasPendingAttachments)
            {
                await WriteResult(body, executionResult);
                return;
            }

            await WriteMultipart(response, executionResult, outgoingAttachments);
        }

        static async Task WriteMultipart(HttpResponse response, ExecutionResult result, OutgoingAttachments attachments)
        {
            var httpContents = new List<HttpContent>();
            try
            {
                using var multipart = new MultipartFormDataContent();
                var serializedResult = JsonConvert.SerializeObject(result);
                multipart.Add(new StringContent(serializedResult));

                foreach (var attachment in attachments.Inner)
                {
                    httpContents.Add(await AddAttachment(attachment, multipart));
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

        static async Task<HttpContent> AddAttachment(KeyValuePair<string, Outgoing> attachment, MultipartFormDataContent multipartContent)
        {
            var outgoing = attachment.Value;
            var httpContent = await outgoing.ContentBuilder();
            if (outgoing.Headers != null)
            {
                foreach (var (key, value) in outgoing.Headers)
                {
                    httpContent.Headers.Add(key, value);
                }
            }

            multipartContent.Add(httpContent, attachment.Key, attachment.Key);
            return httpContent;
        }

        static async Task WriteResult(Stream stream, ExecutionResult result)
        {
            await using var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}