static class QueryRunner
{
    static DocumentExecuter executer = new();

    public static async Task<AttachmentExecutionResult> ExecuteQuery(
        string query,
        ServiceCollection services,
        IIncomingAttachments incomingAttachments)
    {
        query = query.Replace("'", "\"");
        await using var provider = services.BuildServiceProvider();
        using var schema = new Schema(provider);
        var options = new ExecutionOptions
        {
            Schema = schema,
            Query = query
        };

        var result = await executer.ExecuteWithAttachments(options, incomingAttachments);
        var executionResult = result.ExecutionResult;
        var errors = executionResult.Errors;
        if (errors == null || errors.Count == 0)
        {
            return result;
        }

        if (errors.Count == 1)
        {
            throw errors.First();
        }

        throw new AggregateException(errors);
    }
}