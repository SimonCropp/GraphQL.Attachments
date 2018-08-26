using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

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
    public async Task Post(CancellationToken cancellation)
    {
        RequestReader.ReadPost(Request, out var query, out var inputs, out var incomingAttachments, out var operationName);
        using (var attachmentContext = new AttachmentContext(incomingAttachments))
        {
            await Execute(cancellation, query, operationName, attachmentContext, inputs).ConfigureAwait(false);
        }
    }

    [HttpGet]
    public async Task Get(CancellationToken cancellation)
    {
        RequestReader.ReadGet(Request, out var query, out var inputs, out var operationName);
        using (var attachmentContext = new AttachmentContext())
        {
            await Execute(cancellation, query, operationName, attachmentContext, inputs).ConfigureAwait(false);
        }
    }

    async Task Execute(CancellationToken cancellation, string query, string operationName, AttachmentContext attachmentContext, Inputs inputs)
    {
        var executionOptions = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            OperationName = operationName,
            UserContext = attachmentContext,
            Inputs = inputs,
            CancellationToken = cancellation,
#if (DEBUG)
            ThrowOnUnhandledException = true,
            ExposeExceptions = true,
            EnableMetrics = true,
#endif
        };

        var result = await executer.ExecuteAsync(executionOptions).ConfigureAwait(false);
        await ResponseWriter.WriteResult(attachmentContext, Response, result);
    }

}