using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Instrumentation;
using GraphQL.Types;

class AttachmentMiddleware
{
    AttachmentContext attachmentContext;

    public AttachmentMiddleware(AttachmentContext attachmentContext)
    {
        this.attachmentContext = attachmentContext;
    }

    public async Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
    {
        if (context.Arguments == null)
        {
            context.Arguments = new Dictionary<string, object>();
        }
        context.SetAttachmentContext(attachmentContext);
        try
        {
            return await next(context).ConfigureAwait(false);
        }
        finally
        {
            context.RemoveAttachmentContext();
        }
    }
}