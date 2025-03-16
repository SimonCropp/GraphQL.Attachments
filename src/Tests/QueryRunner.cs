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
        if (executionResult.Errors == null || executionResult.Errors.Count == 0)
        {
            return result;
        }

        if (executionResult.Errors.Count == 1)
        {
            throw executionResult.Errors.First();
        }

        throw new AggregateException(executionResult.Errors);
    }
}