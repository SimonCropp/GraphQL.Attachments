using GraphQL;
using GraphQL.Attachments;
using GraphQL.SystemTextJson;
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
    static GraphQLSerializer writer = new(indent: true);

    public GraphQlController(ISchema schema, IDocumentExecuter executer)
    {
        this.schema = schema;
        this.executer = executer;
    }

    #region ControllerPost

    [HttpPost]
    public async Task Post(CancellationToken cancellation)
    {
        var result = await RequestReader.ReadPost(Request, cancellation);
        await Execute(
            result.query,
            result.operation,
            result.attachments,
            result.inputs,
            cancellation);
    }

    #endregion

    #region ControllerGet

    [HttpGet]
    public Task Get(CancellationToken cancellation)
    {
        var (query, inputs, operation) = RequestReader.ReadGet(Request);
        return Execute(query, operation, null, inputs, cancellation);
    }

    #endregion

    async Task Execute(
        string query,
        string operation,
        IIncomingAttachments incomingAttachments,
        Inputs inputs,
        CancellationToken cancellation)
    {
        ExecutionOptions executionOptions = new()
        {
            Schema = schema,
            Query = query,
            OperationName = operation,
            Variables = inputs,
            CancellationToken = cancellation,
#if (DEBUG)
            ThrowOnUnhandledException = true,
            EnableMetrics = true,
#endif
        };

        #region ExecuteWithAttachments

        var result = await executer.ExecuteWithAttachments(
            executionOptions,
            incomingAttachments);

        #endregion

        #region ResponseWriter

        await ResponseWriter.WriteResult(writer, Response, result, cancellation);

        #endregion
    }
}

#endregion