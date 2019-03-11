using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;

static class ArgumentTypeCacheBag
{
    const string key = "GraphQL.Attachments.AttachmentContext";

    public static AttachmentContext GetAttachmentContext(this ResolveFieldContext context)
    {
        return GetAttachmentContext(context.UserContext);
    }

    public static void SetAttachmentContext(this ExecutionOptions options, AttachmentContext attachmentContext)
    {
        if (options.UserContext == null)
        {
            options.UserContext = new Dictionary<string, object>
            {
                {key, attachmentContext}
            };
            return;
        }

        UserContextAsDictionary(options.UserContext).Add(key, attachmentContext);
    }

    public static AttachmentContext GetAttachmentContext<T>(this ResolveFieldContext<T> context)
    {
        return GetAttachmentContext(context.UserContext);
    }

    static AttachmentContext GetAttachmentContext(object userContext)
    {
        var dictionary = UserContextAsDictionary(userContext);

        if (dictionary.TryGetValue(key, out var result))
        {
            return (AttachmentContext) result;
        }

        throw new Exception($"Could not extract {nameof(AttachmentContext)} from ResolveFieldContext.UserContext. It is possible {nameof(AttachmentsExtensions)}.{nameof(AttachmentsExtensions.ExecuteWithAttachments)} was not used.");
    }

    static IDictionary<string, object> UserContextAsDictionary(object userContext)
    {
        if (userContext is IDictionary<string, object> dictionary)
        {
            return dictionary;
        }
        throw NotDictionary();
    }

    static Exception NotDictionary()
    {
        return new Exception("Expected UserContext to be of type IDictionary<string, object>.");
    }
}