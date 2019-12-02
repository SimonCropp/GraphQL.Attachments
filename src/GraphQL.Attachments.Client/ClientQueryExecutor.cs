using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

        public Task ExecutePost(string query, Action<Stream> resultAction, Action<Attachment>? attachmentAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            Guard.AgainstNull(nameof(resultAction), resultAction);
            return ExecutePost(new PostRequest(query), resultAction, attachmentAction, cancellation);
        }

        public Task<QueryResult> ExecutePost(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecutePost(new PostRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecutePost(PostRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            var queryResult = new QueryResult();
            await ExecutePost(
                request,
                resultAction: stream => queryResult.ResultStream = stream,
                attachmentAction: attachment => queryResult.Attachments.Add(attachment.Name, attachment),
                cancellation);
            return queryResult;
        }

        public async Task ExecutePost(PostRequest request, Action<Stream> resultAction, Action<Attachment>? attachmentAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            Guard.AgainstNull(nameof(resultAction), resultAction);
            var content = new MultipartFormDataContent();
            content.AddQueryAndVariables(request.Query, request.Variables, request.OperationName);

            if (request.Action != null)
            {
                var postContext = new PostContext(content);
                request.Action?.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation);

            await response.ProcessResponse(resultAction, attachmentAction, cancellation);
        }

        public Task ExecuteGet(string query, Action<Stream> resultAction, Action<Attachment>? attachmentAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            Guard.AgainstNull(nameof(resultAction), resultAction);
            return ExecuteGet(new GetRequest(query), resultAction, attachmentAction, cancellation);
        }

        public Task<QueryResult> ExecuteGet(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecuteGet(new GetRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecuteGet(GetRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            var queryResult = new QueryResult();
            await ExecuteGet(request,
                resultAction: stream => queryResult.ResultStream = stream,
                attachmentAction: attachment => queryResult.Attachments.Add(attachment.Name, attachment), cancellation);
            return queryResult;
        }

        public async Task ExecuteGet(GetRequest request, Action<Stream> resultAction, Action<Attachment>? attachmentAction = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            Guard.AgainstNull(nameof(resultAction), resultAction);
            var compressed = Compress.Query(request.Query);
            var variablesString = GraphQlRequestAppender.ToJson(request.Variables);
            var getUri = UriBuilder.GetUri(uri, variablesString, compressed, request.OperationName);

            var getRequest = new HttpRequestMessage(HttpMethod.Get, getUri);
            request.HeadersAction?.Invoke(getRequest.Headers);
            var response = await client.SendAsync(getRequest, cancellation);
            await response.ProcessResponse(resultAction, attachmentAction, cancellation);
        }
    }
}