using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

static class QueryExecutor
{
    static DocumentExecuter executer = new DocumentExecuter();

    public static async Task<AttachmentExecutionResult> ExecuteQuery(string queryString, ServiceCollection services, Inputs inputs = null)
    {
        queryString = queryString.Replace("'", "\"");
        using (var provider = services.BuildServiceProvider())
        using (var schema = new Schema(new FuncDependencyResolver(provider.GetRequiredService)))
        {
            var executionOptions = new ExecutionOptions
            {
                Schema = schema,
                Inputs = inputs,
                Query = queryString
            };

            var result = await executer.ExecuteWithAttachments(executionOptions);
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
}