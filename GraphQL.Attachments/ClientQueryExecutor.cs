using System;
using System.Collections.Generic;
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

        public static Task<HttpResponseMessage> ExecutePost(HttpClient client, string query = null, object variables = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(client), client);
            query = CompressQuery(query);
            var body = new
            {
                query,
                variables
            };
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(ToJson(body), Encoding.UTF8, "application/json")
            };
            headerAction?.Invoke(request.Headers);
            return client.SendAsync(request, cancellation);
        }

        public static async Task<HttpResponseMessage> ExecuteMultiFormPost(HttpClient client, string query, object variables = null, Dictionary<string, byte[]> attachments = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(client), client);

            using (var content = new MultipartFormDataContent())
            {
                if (query != null)
                {
                    content.Add(new StringContent(query), "query");
                }
                if (variables != null)
                {
                    content.Add(new StringContent(ToJson(variables)), "variables");
                }
                List<ByteArrayContent> files;
                if (attachments == null)
                {
                    files = new List<ByteArrayContent>();
                }
                else
                {
                    files = new List<ByteArrayContent>(attachments.Count);
                    foreach (var attachment in attachments)
                    {
                        var file = new ByteArrayContent(attachment.Value);
                        content.Add(file, attachment.Key, attachment.Key);
                        files.Add(file);
                    }
                }

                try
                {
                    return await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);
                }
                finally
                {
                    foreach (var file in files)
                    {
                        file.Dispose();
                    }
                }
            }
        }

        public static Task<HttpResponseMessage> ExecuteGet(HttpClient client, string query = null, object variables = null, Action<HttpHeaders> headerAction = null)
        {
            Guard.AgainstNull(nameof(client), client);
            var compressed = CompressQuery(query);
            var variablesString = ToJson(variables);
            var getUri = $"{uri}?query={compressed}&variables={variablesString}";
            var request = new HttpRequestMessage(HttpMethod.Get, getUri);
            headerAction?.Invoke(request.Headers);
            return client.SendAsync(request);
        }

        static string ToJson(object target)
        {
            if (target == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(target);
        }

        static string CompressQuery(string query)
        {
            if (query == null)
            {
                return "";
            }

            return Compress.Query(query);
        }
    }
}