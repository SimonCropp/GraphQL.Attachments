using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class ClientQueryExecutor
    {
        HttpClient client;
        string uri;

        public ClientQueryExecutor(HttpClient client, string uri = "graphql")
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);

            this.client = client;
            this.uri = uri;
        }

        public Task ExecutePost(string query, QueryResponseHandler handler, CancellationToken cancellation = default)
        {
            return ExecutePost(new PostRequest(query), handler, cancellation);
        }

        public Task<QueryResult> ExecutePost(string query, CancellationToken cancellation = default)
        {
            return ExecutePost(new PostRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecutePost(PostRequest request, CancellationToken cancellation = default)
        {
            var queryResult = new QueryResult();
            var handler = new QueryResponseHandler(stream => queryResult.ResultStream = stream)
            {
                AttachmentAction = attachment => queryResult.Attachments.Add(attachment.Name, attachment)
            };
            await ExecutePost(request, handler, cancellation)
                .ConfigureAwait(false);
            return queryResult;
        }

        public async Task ExecutePost(PostRequest request, QueryResponseHandler handler, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(handler), handler);
            var content = new MultipartFormDataContent();
            AddQueryAndVariables(content, request.Query, request.Variables, request.OperationName);

            if (request.Action != null)
            {
                var postContext = new PostContext(content);
                request.Action?.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);

            await ResponseParser.ProcessResponse(response, handler, cancellation).ConfigureAwait(false);
        }

        public Task ExecuteGet(string query, QueryResponseHandler handler, CancellationToken cancellation = default)
        {
            return ExecuteGet(new GetRequest(query), handler, cancellation);
        }

        public Task<QueryResult> ExecuteGet(string query, CancellationToken cancellation = default)
        {
            return ExecuteGet(new GetRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecuteGet(GetRequest request, CancellationToken cancellation = default)
        {
            var queryResult = new QueryResult();
            var handler = new QueryResponseHandler(stream => queryResult.ResultStream = stream)
            {
                AttachmentAction = attachment => queryResult.Attachments.Add(attachment.Name, attachment)
            };
            await ExecuteGet(request, handler, cancellation)
                .ConfigureAwait(false);
            return queryResult;
        }

        public async Task ExecuteGet(GetRequest request, QueryResponseHandler handler, CancellationToken cancellation = default)
        {
            var compressed = Compress.Query(request.Query);
            var variablesString = ToJson(request.Variables);
            var getUri = GetUri(variablesString, compressed, request.OperationName);

            var getRequest = new HttpRequestMessage(HttpMethod.Get, getUri);
            request.HeadersAction?.Invoke(getRequest.Headers);
            var response = await client.SendAsync(getRequest, cancellation).ConfigureAwait(false);
            await ResponseParser.ProcessResponse(response, handler, cancellation).ConfigureAwait(false);
        }

        string GetUri(string variablesString, string compressed, string operationName)
        {
            var getUri= $"{uri}?query={compressed}";

            if (variablesString != null)
            {
                getUri += $"&variables={variablesString}";
            }

            if (operationName != null)
            {
                getUri += $"&operationName={operationName}";
            }

            return getUri;
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