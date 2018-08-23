using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public static class ClientQueryExecutor
    {
        static string uri = "graphql";

        public static void SetQueryUri(string uri)
        {
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);
            ClientQueryExecutor.uri = uri;
        }

        public static Task<HttpResponseMessage> ExecutePost(HttpClient client, string query, object variables = null, string operationName = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            query = Compress.Query(query);
            var body = new
            {
                query,
                variables,
                operationName
            };
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(ToJson(body), Encoding.UTF8, "application/json")
            };
            headerAction?.Invoke(request.Headers);
            return client.SendAsync(request, cancellation);
        }

        public static async Task<HttpResponseMessage> ExecuteMultiFormPost(HttpClient client, string query, object variables = null, string operationName = null, Dictionary<string, Func<Stream>> attachments = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(query), query);

            using (var content = new MultipartFormDataContent())
            {
                AddQueryAndVariables(content, query, variables, operationName);

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        var file = new StreamContent(attachment.Value());
                        content.Add(file, attachment.Key, attachment.Key);
                    }
                }

                headerAction?.Invoke(content.Headers);
                return await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);
            }
        }

        public static async Task<HttpResponseMessage> ExecuteMultiFormPost(HttpClient client, string query, object variables = null, string operationName = null, Dictionary<string, byte[]> attachments = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(query), query);

            using (var content = new MultipartFormDataContent())
            {
                AddQueryAndVariables(content, query, variables, operationName);

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        var file = new ByteArrayContent(attachment.Value);
                        content.Add(file, attachment.Key, attachment.Key);
                    }
                }

                headerAction?.Invoke(content.Headers);
                return await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);
            }
        }

        public static Task<HttpResponseMessage> ExecuteGet(HttpClient client, string query = null, object variables = null, Action<HttpHeaders> headerAction = null)
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            var compressed = Compress.Query(query);
            var variablesString = ToJson(variables);
            var getUri = $"{uri}?query={compressed}&variables={variablesString}";
            var request = new HttpRequestMessage(HttpMethod.Get, getUri);
            headerAction?.Invoke(request.Headers);
            return client.SendAsync(request);
        }

        static void AddQueryAndVariables(MultipartFormDataContent content, string query, object variables, string operationName)
        {
            content.Add(new StringContent(query), "query");

            if (operationName != null)
            {
                content.Add(new StringContent(operationName), "operationName");
            }

            if (variables != null)
            {
                content.Add(new StringContent(ToJson(variables)), "variables");
            }
        }

        static string ToJson(object target)
        {
            if (target == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(target);
        }
    }
}