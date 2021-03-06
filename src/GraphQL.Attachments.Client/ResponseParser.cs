﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public static class ResponseParser
    {
        public static async Task<QueryResult> ProcessResponse(this HttpResponseMessage response, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(response), response);
            if (!response.IsMultipart())
            {
                return new(await response.Content.ReadAsStreamAsync(), new Dictionary<string, Attachment>(), response.Content.Headers, response.StatusCode);
            }

            var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
            Dictionary<string, Attachment> attachments = new();

            await foreach (var attachment in ReadAttachments(multipart).WithCancellation(cancellation))
            {
                attachments.Add(attachment.Name, attachment);
            }

            return new(await ProcessBody(multipart), attachments, response.Content.Headers, response.StatusCode);
        }

        static async IAsyncEnumerable<Attachment> ReadAttachments(MultipartMemoryStreamProvider multipart)
        {
            foreach (var content in multipart.Contents.Skip(1))
            {
                var name = content.Headers.ContentDisposition!.Name!;
                var stream = await content.ReadAsStreamAsync();
                yield return new
                (
                    name: name,
                    stream: stream,
                    headers: content.Headers
                );
            }
        }

        static Task<Stream> ProcessBody(MultipartStreamProvider multipart)
        {
            var first = multipart.Contents.FirstOrDefault();
            if (first == null)
            {
                throw new("Expected the multipart response have at least one part which contains the GraphQL response data.");
            }

            var name = first.Headers.ContentDisposition?.Name;
            if (name == null)
            {
                throw new("Expected the first part in the multipart response to be named.");
            }

            return first.ReadAsStreamAsync();
        }
    }
}