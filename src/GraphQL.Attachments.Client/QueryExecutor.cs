namespace GraphQL.Attachments;

public class QueryExecutor
{
    HttpClient client;
    string uri;

    public QueryExecutor(HttpClient client, string uri = "graphql")
    {
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
        using MultipartFormDataContent content = new();
        content.AddQueryAndVariables(request.Query, request.Variables, request.OperationName);

        if (request.Action != null)
        {
            PostContext postContext = new(content);
            request.Action?.Invoke(postContext);
            postContext.HeadersAction?.Invoke(content.Headers);
        }

        var response = await client.PostAsync(uri, content, cancellation);
        var result = await response.ProcessResponse(cancellation);
        return new(result.Stream, result.Attachments, response.Content.Headers, response.StatusCode);
    }

    public Task<QueryResult> ExecuteGet(string query, CancellationToken cancellation = default)
    {
        Guard.AgainstNullWhiteSpace(nameof(query), query);
        return ExecuteGet(new GetRequest(query), cancellation);
    }

    public async Task<QueryResult> ExecuteGet(GetRequest request, CancellationToken cancellation = default)
    {
        var compressed = Compress.Query(request.Query);
        var variablesString = RequestAppender.ToJson(request.Variables);
        var getUri = UriBuilder.GetUri(uri, variablesString, compressed, request.OperationName);

        using HttpRequestMessage getRequest = new(HttpMethod.Get, getUri);
        request.HeadersAction?.Invoke(getRequest.Headers);
        var response = await client.SendAsync(getRequest, cancellation);
        return await response.ProcessResponse(cancellation);
    }
}