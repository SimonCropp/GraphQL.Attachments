using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.Extensions.DependencyInjection;

static class QueryExecutor
{
    public static async Task<object> ExecuteQuery(string queryString, ServiceCollection services, Inputs inputs = null)
    {
        queryString = queryString.Replace("'", "\"");
        using (var provider = services.BuildServiceProvider())
        using (var schema = new Schema(new FuncDependencyResolver(provider.GetRequiredService)))
        {
            var documentExecuter = new DocumentExecuter();

            var executionOptions = new ExecutionOptions
            {
                Schema = schema,
                Inputs = inputs,
                UserContext = new AttachmentContext(),
                Query = queryString
            };

            var executionResult = await documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if (executionResult.Errors != null && executionResult.Errors.Any())
            {
                if (executionResult.Errors.Count == 1)
                {
                    throw executionResult.Errors.First();
                }

                throw new AggregateException(executionResult.Errors);
            }

            return executionResult.Data;
        }
    }
}