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

        public static void ReadGet(HttpRequest request, out string query, out Inputs inputs, out string operationName)
        {
            ReadParams(request.Query.TryGetValue, out query, out inputs, out operationName);
        }

        public static void ReadPost(HttpRequest request, out string query, out Inputs inputs, out IIncomingAttachments attachments, out string operationName)
        {
            if (request.HasFormContentType)
            {
                ReadForm(request, out query, out inputs, out attachments, out operationName);
                return;
            }

            attachments = new IncomingAttachments();
            ReadBody(request, out query, out inputs, out operationName);
        }

        public class PostBody
        {
            public string OperationName;
            public string Query;
            public JObject Variables;
        }

        static void ReadBody(HttpRequest request, out string query, out Inputs inputs, out string operation)
        {
            using (var streamReader = new StreamReader(request.Body))
            using (var textReader = new JsonTextReader(streamReader))
            {
                var postBody = serializer.Deserialize<PostBody>(textReader);
                query = postBody.Query;
                inputs = postBody.Variables.ToInputs();
                operation = postBody.OperationName;
            }
        }

        static void ReadForm(HttpRequest request, out string query, out Inputs inputs, out IIncomingAttachments attachments, out string operationName)
        {
            ReadParams(request.Form.TryGetValue, out query, out inputs, out operationName);

            attachments = new IncomingAttachments(request.Form.Files.ToDictionary(x => x.FileName, x =>
            {
                return new AttachmentStream(x.OpenReadStream(), x.Length, x.Headers);
            }));
        }

        delegate bool TryGetValue(string key, out StringValues value);

        static void ReadParams(TryGetValue tryGetValue, out string query, out Inputs inputs, out string operationName)
        {
            if (!tryGetValue("query", out var queryValues))
            {
                throw new Exception("Expected to find a form value named 'query'.");
            }

            if (queryValues.Count != 1)
            {
                throw new Exception("Expected 'query' to have a single value.");
            }

            query = queryValues.ToString();

            inputs = GetInputs(tryGetValue);

            operationName = null;
            if (tryGetValue("operation", out var operationValues))
            {
                if (operationValues.Count != 1)
                {
                    throw new Exception("Expected 'operation' to have a single value.");
                }

                operationName = operationValues.ToString();
            }
        }

        static Inputs GetInputs(TryGetValue tryGetValue)
        {
            if (tryGetValue("variables", out var variablesValues))
            {
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
            }

            return new Inputs();
        }
    }
}