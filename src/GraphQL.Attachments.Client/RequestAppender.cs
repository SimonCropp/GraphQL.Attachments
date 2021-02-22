﻿using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public static class RequestAppender
    {
        public static void AddQueryAndVariables(this MultipartFormDataContent content, string query, object? variables = null, string? operation = null)
        {
            Guard.AgainstNull(nameof(content), content);
            content.Add(new StringContent(query), "query");

            if (operation != null)
            {
                content.Add(new StringContent(operation), "operationName");
            }

            if (variables != null)
            {
                content.Add(new StringContent(ToJson(variables)), "variables");
            }
        }

        public static void AddContent(this MultipartFormDataContent content, string name, Stream value)
        {
            Guard.AgainstNull(nameof(content), content);
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            StreamContent file = new(value);
            content.Add(file, name, name);
        }

        public static void AddContent(this MultipartFormDataContent content, string name, byte[] value)
        {
            Guard.AgainstNull(nameof(content), content);
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            ByteArrayContent file = new(value);
            content.Add(file, name, name);
        }

        public static void AddContent(this MultipartFormDataContent content, string name, string value)
        {
            Guard.AgainstNull(nameof(content), content);
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            StringContent file = new(value);
            content.Add(file, name, name);
        }

        internal static string ToJson(object? target)
        {
            if (target == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(target);
        }
    }
}