using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    public Task Post(CancellationToken cancellation)
    {
        RequestReader.ReadPost(Request, out var query, out var inputs, out var incomingAttachments, out var operationName);
        var attachmentContext = new AttachmentContext(incomingAttachments);
        return Execute(cancellation, query, operationName, attachmentContext, inputs);
    }

    [HttpGet]
    public Task Get(CancellationToken cancellation)
    {
        RequestReader.ReadGet(Request, out var query, out var inputs, out var operationName);
        var attachmentContext = new AttachmentContext();
        return Execute(cancellation, query, operationName, attachmentContext, inputs);
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

        SetBadRequestIfErrors(result);

        using (var streamWriter = new StreamWriter(Response.Body, Encoding.UTF8, 1024, true))
        {
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(result)).ConfigureAwait(false);
        }
    }

    void SetBadRequestIfErrors(ExecutionResult result)
    {
        if (result.Errors?.Count > 0)
        {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
}