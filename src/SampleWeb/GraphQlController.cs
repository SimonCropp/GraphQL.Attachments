using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

#region Controller

[Route("[controller]")]
[ApiController]
public class GraphQlController :
    ControllerBase
{
    IDocumentExecuter executer;
    ISchema schema;

    public GraphQlController(ISchema schema, IDocumentExecuter executer)
    {
        this.schema = schema;
        this.executer = executer;
    }

    #region ControllerPost
    [HttpPost]
    public Task Post(CancellationToken cancellation)
    {
        RequestReader.ReadPost(Request, out var query, out var inputs, out var incomingAttachments, out var operation);
        return Execute(query, operation, incomingAttachments, inputs, cancellation);
    }
    #endregion

    #region ControllerGet
    [HttpGet]
    public Task Get(CancellationToken cancellation)
    {
        RequestReader.ReadGet(Request, out var query, out var inputs, out var operation);
        return Execute(query, operation, null, inputs,cancellation);
    }
    #endregion

    async Task Execute(string query, string operation, IIncomingAttachments incomingAttachments, Inputs inputs, CancellationToken cancellation)
    {
        var executionOptions = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            OperationName = operation,
            Inputs = inputs,
            CancellationToken = cancellation,
#if (DEBUG)
            ThrowOnUnhandledException = true,
            ExposeExceptions = true,
            EnableMetrics = true,
#endif
        };

        #region ExecuteWithAttachments
        var result = await executer.ExecuteWithAttachments(executionOptions, incomingAttachments);
        #endregion
        #region ResponseWriter
        await ResponseWriter.WriteResult(Response, result);
        #endregion
    }
}

#endregion