using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;

static class ArgumentTypeCacheBag
{
    public static AttachmentContext GetAttachmentContext(this ResolveFieldContext context)
    {
        return GetAttachmentContext(context.UserContext);
    }

    public static AttachmentContext GetAttachmentContext<T>(this ResolveFieldContext<T> context)
    {
        return GetAttachmentContext(context.UserContext);
    }

    static AttachmentContext GetAttachmentContext(object userContext)
    {
        var dictionary = UserContextAsDictionary(userContext);

        if (dictionary.TryGetValue("GraphQL.Attachments.AttachmentContext", out var result))
        {
            return (AttachmentContext) result;
        }

        throw new Exception($"Could not extract {nameof(AttachmentContext)} from ResolveFieldContext.Arguments. It is possible {nameof(AttachmentsExtensions)}.{nameof(AttachmentsExtensions.ExecuteWithAttachments)} was not used.");
    }

    static IDictionary<string, object> UserContextAsDictionary(object userContext)
    {
        if (userContext is IDictionary<string, object> dictionary)
        {
            return dictionary;
        }
        throw new Exception("Expected UserContext to be of type IDictionary<string, object>.");
    }
}