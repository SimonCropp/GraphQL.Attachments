using System;
using System.Net.Http;
using System.Net.Http.Headers;

static class HttpExtensions
{
    public static bool IsMultipart(this HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.ContentType;
        return string.Equals(contentType?.MediaType, "multipart/form-data", StringComparison.OrdinalIgnoreCase);
    }

    public static void MergeHeaders(this HttpHeaders target, HttpHeaders source)
    {
        if (source == null)
        {
            return;
        }
        foreach (var header in source)
        {
            target.Add(header.Key, header.Value);
        }
    }
}