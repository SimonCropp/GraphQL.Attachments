namespace GraphQL.Attachments;

public class QueryExecutor
{
    HttpClient client;
    string uri;

    public QueryExecutor(HttpClient client, string uri = "graphql")
    {
        Guard.AgainstNullWhiteSpace(uri);

        this.client = client;
        this.uri = uri;
    }

    public Task<QueryResult> ExecutePost(string query, Cancel cancel = default)
    {
        Guard.AgainstNullWhiteSpace(query);
        return ExecutePost(new PostRequest(query), cancel);
    }

    public async Task<QueryResult> ExecutePost(PostRequest request, Cancel cancel = default)
    {
        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(request.Query, request.Variables, request.OperationName);

        if (request.Action != null)
        {
            var postContext = new PostContext(content);
            request.Action?.Invoke(postContext);
            postContext.HeadersAction?.Invoke(content.Headers);
        }

        var response = await client.PostAsync(uri, content, cancel);
        var result = await response.ProcessResponse(cancel);
        return new(result.Stream, result.Attachments, response.Content.Headers, response.Headers, response.StatusCode);
    }

    public Task<QueryResult> ExecuteGet(string query, Cancel cancel = default)
    {
        Guard.AgainstNullWhiteSpace(query);
        return ExecuteGet(new GetRequest(query), cancel);
    }

    public async Task<QueryResult> ExecuteGet(GetRequest request, Cancel cancel = default)
    {
        var compressed = Compress.Query(request.Query);
        var variablesString = RequestAppender.ToJson(request.Variables);
        var getUri = UriBuilder.GetUri(uri, variablesString, compressed, request.OperationName);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, getUri);
        request.HeadersAction?.Invoke(getRequest.Headers);
        var response = await client.SendAsync(getRequest, cancel);
        return await response.ProcessResponse(cancel);
    }
}