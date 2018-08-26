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

        public async Task<QueryResult> ExecutePost(PostRequest request, CancellationToken cancellation = default)
        {
            var content = new MultipartFormDataContent();
            AddQueryAndVariables(content, request.Query, request.Variables, request.OperationName);

            if (request.Action != null)
            {
                var postContext = new PostContext(content);
                request.Action.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);

            return await ResponseParser.EvaluateResponse(response, cancellation).ConfigureAwait(false);
        }

        public async Task<QueryResult> ExecuteGet(string query, object variables = null, Action<HttpHeaders> headerAction = null, CancellationToken cancellation = default)
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
            var response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
            return await ResponseParser.EvaluateResponse(response, cancellation).ConfigureAwait(false);
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

    public class PostRequest
    {
        public string Query { get; }

        public PostRequest(string query)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            Query = query;
        }

        public Action<PostContext> Action { get; set; }
        public object Variables { get; set; }
        public string OperationName { get; set; }
    }
}