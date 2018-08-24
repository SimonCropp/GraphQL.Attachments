using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class ClientQueryExecutor
    {
        HttpClient client;
        string uri;

        public ClientQueryExecutor(HttpClient client, string uri= "graphql")
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);

            this.client = client;
            this.uri = uri;
        }

        public async Task<QueryResult> ExecutePost(string query, object variables = null, string operationName = null, Action<PostContext> action = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            var content = new MultipartFormDataContent();

            AddQueryAndVariables(content, query, variables, operationName);

            if (action != null)
            {
                var postContext = new PostContext(content);
                action.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);

            var queryResult = new QueryResult(response);
            if (response.Content.Headers.ContentType.MediaType == "multipart/form-data")
            {
                var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
                foreach (var multipartContent in multipart.Contents)
                {
                    var name = multipartContent.Headers.ContentDisposition.Name;
                    if (name == null)
                    {
                        queryResult.ResultStream = await multipartContent.ReadAsStreamAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        queryResult.Attachments[name] = await multipartContent.ReadAsStreamAsync().ConfigureAwait(false);
                    }
                }

                if (queryResult.ResultStream == null)
                {
                    throw new Exception("Expected the multipart response top contain a single un-named part which contains the graphql response data.");
                }
            }
            else
            {
                queryResult.ResultStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            return queryResult;
        }

        public Task<HttpResponseMessage> ExecuteGet(string query, object variables = null, Action<HttpHeaders> headerAction = null)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            var compressed = Compress.Query(query);
            var variablesString = ToJson(variables);
            string getUri;
            if (variablesString == null)
            {
                getUri = $"{uri}?query={compressed}";
            }
            else
            {
                getUri = $"{uri}?query={compressed}&variables={variablesString}";
            }

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