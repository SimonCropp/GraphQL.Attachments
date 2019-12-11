using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace GraphQL.Attachments
{
    /// <summary>
    /// Handles parsing a <see cref="HttpRequest"/> into the corresponding query, <see cref="Inputs"/>, operation, and <see cref="IIncomingAttachments"/>.
    /// </summary>
    public static class RequestReader
    {
        /// <summary>
        /// Parse a <see cref="HttpRequest"/> Get into the corresponding query, <see cref="Inputs"/>, and operation.
        /// </summary>
        public static (string query, Inputs inputs, string? operation) ReadGet(HttpRequest request)
        {
            Guard.AgainstNull(nameof(request), request);
            return ReadParams(request.Query.TryGetValue);
        }

        /// <summary>
        /// Parse a <see cref="HttpRequest"/> Post into the corresponding query, <see cref="Inputs"/>, operation, and <see cref="IIncomingAttachments"/>.
        /// </summary>
        public static async Task<(string query, Inputs inputs, IIncomingAttachments attachments, string? operation)> ReadPost(HttpRequest request)
        {
            Guard.AgainstNull(nameof(request), request);
            if (request.HasFormContentType)
            {
                return await ReadForm(request);
            }

            var (query, inputs, operation) = await ReadBody(request);
            return (query, inputs, new IncomingAttachments(), operation);
        }

        internal class PostBody
        {
            public string OperationName = null!;
            public string Query = null!;
            public JObject Variables = null!;
        }

        static async Task<(string query, Inputs inputs, string operation)> ReadBody(HttpRequest request)
        {
            var postBody = await JsonSerializer.DeserializeAsync<PostBody>(request.Body);
            return (postBody!.Query, postBody.Variables.ToInputs(), postBody.OperationName);
        }

        static async Task<(string query, Inputs inputs, IIncomingAttachments attachments, string? operation)> ReadForm(HttpRequest request)
        {
            var form = await request.ReadFormAsync();
            var (query, inputs, operation) = ReadParams(form.TryGetValue);

            var streams = form.Files.ToDictionary(
                x => x.FileName,
                x => new AttachmentStream(x.FileName, x.OpenReadStream(), x.Length, x.Headers));
            return (query, inputs, new IncomingAttachments(streams), operation);
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
            if (!tryGetValue("operation", out var operationValues))
            {
                return null;
            }

            if (operationValues.Count == 1)
            {
                return operationValues.ToString();
            }

            throw new Exception("Expected 'operation' to have a single value.");
        }

        static Inputs GetInputs(TryGetValue tryGetValue)
        {
            if (!tryGetValue("variables", out var values))
            {
                return new Inputs();
            }

            if (values.Count != 1)
            {
                throw new Exception("Expected 'variables' to have a single value.");
            }

            var json = values.ToString();
            if (json.Length == 0)
            {
                return new Inputs();
            }
            var variables = JObject.Parse(json);
            return variables.ToInputs();
        }
    }
}