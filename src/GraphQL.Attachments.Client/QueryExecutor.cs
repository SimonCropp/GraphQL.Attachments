using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public class QueryExecutor
    {
        HttpClient client;
        string uri;

        public QueryExecutor(HttpClient client, string uri = "graphql")
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);

            this.client = client;
            this.uri = uri;
        }

        public Task<QueryResult> ExecutePost(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecutePost(new PostRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecutePost(PostRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            Guard.AgainstNull(nameof(request), request);
            using var content = new MultipartFormDataContent();
            content.AddQueryAndVariables(request.Query, request.Variables, request.OperationName);

            if (request.Action != null)
            {
                var postContext = new PostContext(content);
                request.Action?.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation);
            var result = await response.ProcessResponse(cancellation);
            return new QueryResult(result.Stream, result.Attachments, response.Content.Headers);
        }

        public Task<QueryResult> ExecuteGet(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecuteGet(new GetRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecuteGet(GetRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            var compressed = Compress.Query(request.Query);
            var variablesString = RequestAppender.ToJson(request.Variables);
            var getUri = UriBuilder.GetUri(uri, variablesString, compressed, request.OperationName);

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, getUri);
            request.HeadersAction?.Invoke(getRequest.Headers);
            var response = await client.SendAsync(getRequest, cancellation);
            return await response.ProcessResponse(cancellation);
        }
    }
}