namespace GraphQL.Attachments;

public static class ResponseParser
{
    public static async Task<QueryResult> ProcessResponse(this HttpResponseMessage response, Cancel cancel = default)
    {
        if (!response.IsMultipart())
        {
            return new(await response.Content.ReadAsStreamAsync(cancel), new Dictionary<string, Attachment>(), response.Content.Headers, response.Headers, response.StatusCode);
        }

        var multipart = await response.Content.ReadAsMultipartAsync(cancel);
        var attachments = new Dictionary<string, Attachment>();

        await foreach (var attachment in ReadAttachments(multipart).WithCancellation(cancel))
        {
            attachments.Add(attachment.Name, attachment);
        }

        return new(await ProcessBody(multipart), attachments, response.Content.Headers, response.Headers, response.StatusCode);
    }

    static async IAsyncEnumerable<Attachment> ReadAttachments(MultipartMemoryStreamProvider multipart)
    {
        foreach (var content in multipart.Contents.Skip(1))
        {
            var name = content.Headers.ContentDisposition!.Name!;
            var stream = await content.ReadAsStreamAsync();
            yield return new(
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