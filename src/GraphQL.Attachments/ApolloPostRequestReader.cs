using JsonSerializer = System.Text.Json.JsonSerializer;

/// <summary>
/// Handles parsing a <see cref="HttpRequest" /> into the corresponding query, <see cref="Inputs" />, operation, and <see cref="IIncomingAttachments" />.
/// </summary>
public static class ApolloPostRequestReader
{
    static GraphQLSerializer serializer = new(true);

    /// <summary>
    /// Parse a <see cref="HttpRequest" /> Post into the corresponding query, <see cref="Inputs" />, operation, and <see cref="IIncomingAttachments" />.
    /// </summary>
    public static async Task<(string query, Inputs? inputs, IIncomingAttachments attachments, string? operation)> ReadPost(
        HttpRequest request,
        Cancel cancel = default)
    {
        if (request.HasFormContentType)
        {
            return await ReadForm(request, cancel);
        }

        var (query, inputs, operation) = await ReadBody(request.Body);
        return (query, inputs, new IncomingAttachments(), operation);
    }

    static async Task<(string query, Inputs? inputs, string operation)> ReadBody(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var value = await reader.ReadToEndAsync();
        ApolloPostRequestOperation? postBody;
        try
        {
            postBody = JsonSerializer.Deserialize<ApolloPostRequestOperation>(value);
        }
        catch (Exception exception)
        {
            throw new($"Failed to deserialize:{value}", exception);
        }

        return postBody!.ToOperationParts(serializer);
    }

    static async Task<(string query, Inputs? inputs, IIncomingAttachments attachments, string? operation)> ReadForm(
        HttpRequest request,
        Cancel cancel)
    {
        var form = await request.ReadFormAsync(cancel);
        var (query, inputs, operation) = ReadParams(form.TryGetValue);

        var streams = form.Files.ToDictionary(
            _ => _.FileName,
            _ => new AttachmentStream(_.FileName, _.OpenReadStream(), _.Length, _.Headers));
        return (query, inputs, new IncomingAttachments(streams), operation);
    }

    delegate bool TryGetValue(string key, out StringValues value);

    static (string query, Inputs? inputs, string? operation) ReadParams(TryGetValue tryGetValue)
    {
        if (!tryGetValue("operations", out var operationsValues))
        {
            throw new("Expected to find a form value named 'operations'.");
        }

        if (operationsValues.Count != 1)
        {
            throw new("Expected 'operations' to have a single value.");
        }

        var operation = JsonSerializer.Deserialize<ApolloPostRequestOperation>(operationsValues!);

        if (operation == null)
        {
            throw new("Expected 'operations' to not be null");
        }

        return operation.ToOperationParts(serializer);
    }

    public class ApolloPostRequestOperation
    {
        // ReSharper disable InconsistentNaming
        public string operationName { get; init; } = null!;
        public string query { get; init; } = null!;
        public object? variables { get; init; }

        public (string query, Inputs? inputs, string name) ToOperationParts(IGraphQLTextSerializer serializer)
        {
            var inputVariables = variables?.ToString() ?? "{}";
            var inputs = serializer.Deserialize<Inputs>(inputVariables);

            return (query, inputs, operationName);
        }
    }
}