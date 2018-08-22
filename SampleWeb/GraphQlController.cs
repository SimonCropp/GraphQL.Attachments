using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
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

        JObject variables = null;

        if (Request.Form.TryGetValue("variables", out var variablesValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'variables' to have a single value.");
            }
            variables = JObject.Parse(variablesValues.ToString());
        }

        string operation = null;
        if (Request.Form.TryGetValue("operation", out var operationValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'operation' to have a single value.");
            }
            operation = operationValues.ToString();
        }

        return Execute(query, operation, variables, cancellation);
    }

    public class PostBody
    {
        public string OperationName;
        public string Query;
        public JObject Variables;
    }

    [HttpGet]
    public Task<ExecutionResult> Get(
        [FromQuery] string query,
        [FromQuery] string variables,
        [FromQuery] string operationName,
        CancellationToken cancellation)
    {
        var jObject = ParseVariables(variables);
        return Execute(query, operationName, jObject, cancellation);
    }

    async Task<ExecutionResult> Execute(string query, string operationName, JObject variables, CancellationToken cancellation)
    {
        var executionOptions = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            OperationName = operationName,
            Inputs = variables?.ToInputs(),
            CancellationToken = cancellation,
#if (DEBUG)
            ExposeExceptions = true,
            EnableMetrics = true,
#endif
        };

        var result = await executer.ExecuteAsync(executionOptions).ConfigureAwait(false);

        if (result.Errors?.Count > 0)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
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