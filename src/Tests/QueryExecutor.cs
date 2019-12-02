using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.Extensions.DependencyInjection;

static class QueryExecutor
{
    static DocumentExecuter executer = new DocumentExecuter();

    public static async Task<AttachmentExecutionResult> ExecuteQuery(string queryString, ServiceCollection services, IIncomingAttachments incomingAttachments)
    {
        queryString = queryString.Replace("'", "\"");
        await using var provider = services.BuildServiceProvider();
        using var schema = new Schema(new FuncDependencyResolver(provider.GetRequiredService));
        var options = new ExecutionOptions
        {
            Schema = schema,
            Query = queryString
        };

        var result = await executer.ExecuteWithAttachments(options,incomingAttachments);
        var executionResult = result.ExecutionResult;
        if (executionResult.Errors != null && executionResult.Errors.Any())
        {
            if (executionResult.Errors.Count == 1)
            {
                throw executionResult.Errors.First();
            }

            throw new AggregateException(executionResult.Errors);
        }

        return result;
    }
}