using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[Route("[controller]")]
[ApiController]
public class GraphQlController : ControllerBase
{
    IDocumentExecuter executer;
    ISchema schema;

    public GraphQlController(ISchema schema, IDocumentExecuter executer)
    {
        this.schema = schema;
        this.executer = executer;
    }

    [HttpPost]
    public Task<ExecutionResult> Post(CancellationToken cancellation)
    {
        if (!Request.Form.TryGetValue("query", out var queryValues))
        {
            throw new Exception("Expected to find a form value named 'query'.");
        }

        if (queryValues.Count != 1)
        {
            throw new Exception("Expected 'query' to have a single value.");
        }

        var query = queryValues.ToString();

        Inputs inputs;
        if (Request.Form.TryGetValue("variables", out var variablesValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'variables' to have a single value.");
            }
            var variables = JObject.Parse(variablesValues.ToString());
            inputs = variables.ToInputs();
        }
        else
        {
            inputs = new Inputs();
        }
        inputs.Add("attachments", Request.Form.Files);

        string operation = null;
        if (Request.Form.TryGetValue("operation", out var operationValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'operation' to have a single value.");
            }
            operation = operationValues.ToString();
        }
        return Execute(query, operation, cancellation, inputs);
    }


    [HttpGet]
    public Task<ExecutionResult> Get(
        [FromQuery] string query,
        [FromQuery] string variables,
        [FromQuery] string operationName,
        CancellationToken cancellation)
    {
        var jObject = ParseVariables(variables);
        var inputs = jObject?.ToInputs();
        return Execute(query, operationName, cancellation, inputs);
    }

    private async Task<ExecutionResult> Execute(string query, string operationName, CancellationToken cancellation, Inputs inputs)
    {
        var executionOptions = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            OperationName = operationName,
            Inputs = inputs,
            CancellationToken = cancellation,
#if (DEBUG)
            ExposeExceptions = true,
            EnableMetrics = true,
#endif
        };

        var result = await executer.ExecuteAsync(executionOptions).ConfigureAwait(false);

        if (result.Errors?.Count > 0)
        {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }

        return result;
    }

    static JObject ParseVariables(string variables)
    {
        if (variables == null)
        {
            return null;
        }

        try
        {
            return JObject.Parse(variables);
        }
        catch (Exception exception)
        {
            throw new Exception("Could not parse variables.", exception);
        }
    }
}
