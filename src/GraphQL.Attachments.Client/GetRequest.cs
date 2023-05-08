using System.Net.Http.Headers;

namespace GraphQL.Attachments;

public class GetRequest
{
    public string Query { get; }

    public GetRequest(string query)
    {
        Guard.AgainstNullWhiteSpace(query);
        Query = query;
    }

    public Action<HttpRequestHeaders>? HeadersAction { get; set; }
    public object? Variables { get; set; }
    public string? OperationName { get; set; }
}