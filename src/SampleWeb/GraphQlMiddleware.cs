using System.Net;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.SystemTextJson;
using GraphQL.Types;

public class GraphQlMiddleware :
    IMiddleware
{
    IDocumentExecuter executer;
    ISchema schema;
    static HttpReaderWriter readerWriter = new(new GraphQLSerializer(indent: true));

    public GraphQlMiddleware(ISchema schema, IDocumentExecuter executer)
    {
        this.schema = schema;
        this.executer = executer;
    }

    #region Invoke

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var cancel = context.RequestAborted;
        var response = context.Response;
        var request = context.Request;
        var isGet = HttpMethods.IsGet(request.Method);
        var isPost = HttpMethods.IsPost(request.Method);

        if (isGet)
        {
            var (query, inputs, operation) = readerWriter.ReadGet(request);
            await Execute(response, query, operation, null, inputs, cancel);
            return;
        }

        if (isPost)
        {
            var (query, inputs, attachments, operation) = await readerWriter.ReadPost(request, cancel);
            await Execute(response, query, operation, attachments, inputs, cancel);
            return;
        }

        response.Headers.Allow = "GET, POST";
        response.StatusCode = (int) HttpStatusCode.BadRequest;
    }

    #endregion

    async Task Execute(
        HttpResponse response,
        string query,
        string? operation,
        IIncomingAttachments? attachments,
        Inputs? inputs,
        Cancel cancel)
    {
        var options = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            OperationName = operation,
            Variables = inputs,
            CancellationToken = cancel,
#if (DEBUG)
            ThrowOnUnhandledException = true,
            EnableMetrics = true,
#endif
        };

        #region ExecuteWithAttachments

        var result = await executer.ExecuteWithAttachments(options, attachments);

        #endregion

        #region ResponseWriter

        await readerWriter.WriteResult(response, result, cancel);

        #endregion
    }
}