namespace GraphQL.Attachments;

public static class ResponseParser
{
    public static async Task<QueryResult> ProcessResponse(this HttpResponseMessage response, Cancel cancel = default)
    {
        if (!response.IsMultipart())
        {
            return new(await response.Content.ReadAsStreamAsync(cancel), new Dictionary<string, Attachment>(), response.Content.Headers, response.Headers, response.StatusCode);
        }

        if (!response.Content.TryGetMultipartBoundary(out var boundary))
        {
            throw new("Expected the multipart response to declare a boundary.");
        }

        var parts = await ReadParts(response, boundary, cancel);
        var attachments = new Dictionary<string, Attachment>();

        foreach (var attachment in ReadAttachments(parts))
        {
            attachments.Add(attachment.Name, attachment);
        }

        return new(ProcessBody(parts), attachments, response.Content.Headers, response.Headers, response.StatusCode);
    }

    /// <summary>
    /// Reads every part into memory as the <see cref="HttpContent"/> it arrived as. The reader is
    /// forward-only — a section is readable only until the next one is — so buffering here is what lets
    /// an attachment be read in any order, and after this returns.
    /// </summary>
    static async Task<List<HttpContent>> ReadParts(HttpResponseMessage response, string boundary, Cancel cancel)
    {
        var stream = await response.Content.ReadAsStreamAsync(cancel);
        await using (stream)
        {
            using var reader = new MultipartReader(boundary, stream);
            var parts = new List<HttpContent>();
            while (await reader.ReadNextSectionAsync(cancel) is {} section)
            {
                var content = new ByteArrayContent(await section.ReadAsBytesAsync(cancel));
                foreach (var (key, value) in section.Headers!)
                {
                    content.Headers.TryAddWithoutValidation(key, value);
                }

                parts.Add(content);
            }

            return parts;
        }
    }

    static IEnumerable<Attachment> ReadAttachments(List<HttpContent> parts)
    {
        foreach (var content in parts.Skip(1))
        {
            var name = content.Headers.ContentDisposition!.Name!;
            yield return new(
                name: Unquote(name),
                stream: content.ReadAsStream(),
                headers: content.Headers
            );
        }
    }

    static Stream ProcessBody(List<HttpContent> parts)
    {
        var first = parts.FirstOrDefault();
        if (first == null)
        {
            throw new("Expected the multipart response have at least one part which contains the GraphQL response data.");
        }

        var name = first.Headers.ContentDisposition?.Name;
        if (name == null)
        {
            throw new("Expected the first part in the multipart response to be named.");
        }

        return first.ReadAsStream();
    }

    // A name is quoted on the wire unless every one of its characters is a token character, and the
    // two spell the same name.
    static string Unquote(string name) =>
        name.Trim('"');
}
