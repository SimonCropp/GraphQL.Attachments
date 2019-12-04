using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQL.Attachments
{
    public static class RequestReader
    {
        static JsonSerializer serializer = JsonSerializer.CreateDefault();

        public static (string query, Inputs inputs, string? operation) ReadGet(HttpRequest request)
        {
            Guard.AgainstNull(nameof(request), request);
            return ReadParams(request.Query.TryGetValue);
        }

        public static (string query, Inputs inputs, IIncomingAttachments attachments, string? operation) ReadPost(HttpRequest request)
        {
            Guard.AgainstNull(nameof(request), request);
            if (request.HasFormContentType)
            {
                return ReadForm(request);
            }

            var (query, inputs, operation) = ReadBody(request);
            return (query, inputs, new IncomingAttachments(), operation);
        }

        public class PostBody
        {
            public string OperationName = null!;
            public string Query = null!;
            public JObject Variables = null!;
        }

        static (string query, Inputs inputs, string operation) ReadBody(HttpRequest request)
        {
            using var streamReader = new StreamReader(request.Body);
            using var textReader = new JsonTextReader(streamReader);
            var postBody = serializer.Deserialize<PostBody>(textReader);
            return (postBody!.Query, postBody.Variables.ToInputs(), postBody.OperationName);
        }

        static (string query, Inputs inputs, IIncomingAttachments attachments, string? operation) ReadForm(HttpRequest request)
        {
            var form = request.Form;
            var (query, inputs, operation) = ReadParams(form.TryGetValue);

            var attachmentStreams = form.Files.ToDictionary(
                x => x.FileName,
                x => new AttachmentStream(x.FileName, x.OpenReadStream(), x.Length, x.Headers));
            return (query, inputs, new IncomingAttachments(attachmentStreams), operation);
        }

        delegate bool TryGetValue(string key, out StringValues value);

        static (string query, Inputs inputs, string? operation) ReadParams(TryGetValue tryGetValue)
        {
            if (!tryGetValue("query", out var queryValues))
            {
                throw new Exception("Expected to find a form value named 'query'.");
            }

            if (queryValues.Count != 1)
            {
                throw new Exception("Expected 'query' to have a single value.");
            }

            var operation = ReadOperation(tryGetValue);

            return (queryValues.ToString(), GetInputs(tryGetValue), operation);
        }

        static string? ReadOperation(TryGetValue tryGetValue)
        {
            if (tryGetValue("operation", out var operationValues))
            {
                if (operationValues.Count != 1)
                {
                    throw new Exception("Expected 'operation' to have a single value.");
                }

                return operationValues.ToString();
            }

            return null;
        }

        static Inputs GetInputs(TryGetValue tryGetValue)
        {
            if (!tryGetValue("variables", out var variablesValues))
            {
                return new Inputs();
            }

            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'variables' to have a single value.");
            }

            var json = variablesValues.ToString();
            if (json.Length > 0)
            {
                var variables = JObject.Parse(json);
                return variables.ToInputs();
            }

            return new Inputs();
        }
    }
}