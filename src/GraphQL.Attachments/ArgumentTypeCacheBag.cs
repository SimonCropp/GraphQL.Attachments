using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;

static class ArgumentTypeCacheBag
{
    public static AttachmentContext GetAttachmentContext(this ResolveFieldContext context)
    {
        return GetAttachmentContext(context.Arguments);
    }

    public static void SetAttachmentContext(this ResolveFieldContext context, AttachmentContext cache)
    {
        context.Arguments.Add("GraphQL.Attachments.AttachmentContext", cache);
    }

    public static void RemoveAttachmentContext(this ResolveFieldContext context)
    {
        context.Arguments.Remove("GraphQL.Attachments.AttachmentContext");
    }

    public static AttachmentContext GetAttachmentContext<T>(this ResolveFieldContext<T> context)
    {
        return GetAttachmentContext(context.Arguments);
    }

    static AttachmentContext GetAttachmentContext(Dictionary<string, object> arguments)
    {
        if (arguments.TryGetValue("GraphQL.Attachments.AttachmentContext", out var result))
        {
            return (AttachmentContext) result;
        }

        throw new Exception($"Could not extract {nameof(AttachmentContext)} from ResolveFieldContext.Arguments. It is possible {nameof(AttachmentsExtensions)}.{nameof(AttachmentsExtensions.ExecuteWithAttachments)} was not used.");
    }
}