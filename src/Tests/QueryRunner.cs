using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.Extensions.DependencyInjection;

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
        using Schema schema = new(provider);
        ExecutionOptions options = new()
        {
            Schema = schema,
            Query = query
        };

        var result = await executer.ExecuteWithAttachments(options, incomingAttachments);
        var executionResult = result.ExecutionResult;
        if (executionResult.Errors == null || !executionResult.Errors.Any())
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