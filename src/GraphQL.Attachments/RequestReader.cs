using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace GraphQL.Attachments;

/// <summary>
/// Handles parsing a <see cref="HttpRequest"/> into the corresponding query, <see cref="Inputs"/>, operation, and <see cref="IIncomingAttachments"/>.
/// </summary>
public static class RequestReader
{
    /// <summary>
    /// Parse a <see cref="HttpRequest"/> Get into the corresponding query, <see cref="Inputs"/>, and operation.
    /// </summary>
    public static (string query, Inputs? inputs, string? operation) ReadGet(IGraphQLTextSerializer serializer, HttpRequest request) =>
        ReadParams(serializer, request.Query.TryGetValue);

    /// <summary>
    /// Parse a <see cref="HttpRequest"/> Post into the corresponding query, <see cref="Inputs"/>, operation, and <see cref="IIncomingAttachments"/>.
    /// </summary>
    public static async Task<(string query, Inputs? inputs, IIncomingAttachments attachments, string? operation)> ReadPost(
        IGraphQLTextSerializer serializer,
        HttpRequest request,
        CancellationToken cancellation = default)
    {
        if (request.HasFormContentType)
        {
            return await ReadForm(serializer, request, cancellation);
        }

        var (query, inputs, operation) = await ReadBody(serializer, request.Body, cancellation);
        return (query, inputs, new IncomingAttachments(), operation);
    }

    class PostBody
    {
        public string operationName { get; set; } = null!;
        public string query { get; set; } = null!;
        public Inputs? variables { get; set; }
    }

    internal static async Task<(string query, Inputs? inputs, string operation)> ReadBody(
        IGraphQLTextSerializer serializer,
        Stream stream,
        CancellationToken cancellation)
    {
        var postBody = (await serializer.ReadAsync<PostBody>(stream, cancellationToken: cancellation))!;
        return (postBody.query, postBody.variables, postBody.operationName);
    }

    static async Task<(string query, Inputs? inputs, IIncomingAttachments attachments, string? operation)> ReadForm(
        IGraphQLTextSerializer serializer,
        HttpRequest request,
        CancellationToken cancellation)
    {
        var form = await request.ReadFormAsync(cancellation);
        var (query, inputs, operation) = ReadParams(serializer, form.TryGetValue);

        var streams = form.Files.ToDictionary(
            x => x.FileName,
            x => new AttachmentStream(x.FileName, x.OpenReadStream(), x.Length, x.Headers));
        return (query, inputs, new IncomingAttachments(streams), operation);
    }

    delegate bool TryGetValue(string key, out StringValues value);

    static (string query, Inputs? inputs, string? operation) ReadParams(IGraphQLTextSerializer serializer, TryGetValue tryGetValue)
    {
        if (!tryGetValue("query", out var queryValues))
        {
            throw new("Expected to find a form value named 'query'.");
        }

        if (queryValues.Count != 1)
        {
            throw new("Expected 'query' to have a single value.");
        }

        var operation = ReadOperation(tryGetValue);

        return (queryValues.ToString(), GetInputs(serializer, tryGetValue), operation);
    }

    static string? ReadOperation(TryGetValue tryGetValue)
    {
        if (!tryGetValue("operation", out var operationValues))
        {
            return null;
        }

        if (operationValues.Count == 1)
        {
            return operationValues.ToString();
        }

        throw new("Expected 'operation' to have a single value.");
    }

    static Inputs? GetInputs(IGraphQLTextSerializer serializer, TryGetValue tryGetValue)
    {
        if (!tryGetValue("variables", out var values))
        {
            return null;
        }

        if (values.Count != 1)
        {
            throw new("Expected 'variables' to have a single value.");
        }

        var json = values.ToString();
        if (json.Length == 0)
        {
            return null;
        }

        return serializer.Deserialize<Inputs>(json);
    }
}