using GraphQL;
using GraphQL.Attachments;

static class ArgumentTypeCacheBag
{
    const string key = "GraphQL.Attachments.AttachmentContext";

    public static AttachmentContext GetAttachmentContext(this IResolveFieldContext context) =>
        GetAttachmentContext(context.UserContext);

    public static void SetAttachmentContext(this ExecutionOptions options, AttachmentContext attachmentContext) =>
        UserContextAsDictionary(options.UserContext).Add(key, attachmentContext);

    public static AttachmentContext GetAttachmentContext<T>(this IResolveFieldContext<T> context) =>
        GetAttachmentContext(context.UserContext);

    static AttachmentContext GetAttachmentContext(object userContext)
    {
        var dictionary = UserContextAsDictionary(userContext);

        if (dictionary.TryGetValue(key, out var result))
        {
            return (AttachmentContext) result;
        }

        throw new($"Could not extract {nameof(AttachmentContext)} from ResolveFieldContext.UserContext. It is possible {nameof(AttachmentsExtensions)}.{nameof(AttachmentsExtensions.ExecuteWithAttachments)} was not used.");
    }

    public static IDictionary<string, object> UserContextAsDictionary(object userContext)
    {
        if (userContext is IDictionary<string, object> dictionary)
        {
            return dictionary;
        }

        throw NotDictionary();
    }

    static Exception NotDictionary() => new("Expected UserContext to be of type IDictionary<string, object>.");
}