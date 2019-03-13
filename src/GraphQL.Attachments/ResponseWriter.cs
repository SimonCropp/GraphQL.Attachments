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
    public static class ResponseWriter
    {
        public static async Task WriteResult(HttpResponse httpResponse, AttachmentExecutionResult attachmentExecutionResult)
        {
            var result = attachmentExecutionResult.ExecutionResult;
            var outgoingAttachments = (OutgoingAttachments) attachmentExecutionResult.Attachments;
            var responseBody = httpResponse.Body;
            if (result.Errors?.Count > 0)
            {
                httpResponse.StatusCode = (int) HttpStatusCode.BadRequest;
                await WriteResult(responseBody, result);
                return;
            }

            if (!outgoingAttachments.HasPendingAttachments)
            {
                await WriteResult(responseBody, result);
                return;
            }

            var httpContents = new List<HttpContent>();
            using (var multipartContent = new MultipartFormDataContent())
            {
                var serializedResult = JsonConvert.SerializeObject(result);
                multipartContent.Add(new StringContent(serializedResult));

                foreach (var outgoingAttachment in outgoingAttachments.Inner)
                {
                    var outgoing = outgoingAttachment.Value;
                    var httpContent = await outgoing.ContentBuilder();
                    httpContents.Add(httpContent);
                    if (outgoing.Headers != null)
                    {
                        foreach (var header in outgoing.Headers)
                        {
                            httpContent.Headers.Add(header.Key, header.Value);
                        }
                    }

                    multipartContent.Add(httpContent, outgoingAttachment.Key, outgoingAttachment.Key);
                }

                httpResponse.ContentLength = multipartContent.Headers.ContentLength;
                httpResponse.ContentType = multipartContent.Headers.ContentType.ToString();
                await multipartContent.CopyToAsync(responseBody);
            }

            foreach (var httpContent in httpContents)
            {
                httpContent.Dispose();
            }

            foreach (var cleanup in outgoingAttachments.Inner.Select(x=>x.Value.Cleanup))
            {
                cleanup?.Invoke();
            }
        }

        static async Task WriteResult(Stream stream, ExecutionResult result)
        {
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteAsync(JsonConvert.SerializeObject(result));
            }
        }
    }
}