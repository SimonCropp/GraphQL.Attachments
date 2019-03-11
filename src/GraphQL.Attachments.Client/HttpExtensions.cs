﻿using System;
using System.Net.Http;

namespace GraphQL.Attachments
{
    public static class HttpExtensions
    {
        public static bool IsMultipart(this HttpResponseMessage response)
        {
            Guard.AgainstNull(nameof(response), response);
            var contentType = response.Content.Headers.ContentType;
            return string.Equals(contentType?.MediaType, "multipart/form-data", StringComparison.OrdinalIgnoreCase);
        }
    }
}