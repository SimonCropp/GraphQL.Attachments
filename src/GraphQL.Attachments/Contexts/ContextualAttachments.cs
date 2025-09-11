namespace GraphQL.Attachments;

public static class ContextualAttachments
{
    public static IIncomingAttachments IncomingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment<TSource>(this IResolveFieldContext<TSource> context) =>
        context.IncomingAttachments().Single().Value;

    public static AttachmentStream IncomingAttachment<TSource>(this IResolveFieldContext<TSource> context, string name) =>
        FindSingle(name, context.IncomingAttachments());

    public static IIncomingAttachments IncomingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment(this IResolveFieldContext context) =>
        context.IncomingAttachments().Single().Value;

    public static AttachmentStream IncomingAttachment(this IResolveFieldContext context, string name) =>
        FindSingle(name, context.IncomingAttachments());

    static AttachmentStream FindSingle(string name, IIncomingAttachments attachments)
    {
        if (attachments.TryGetValue(name, out var attachment))
        {
            return attachment;
        }

        throw new($"Attachment not found: '{name}'");
    }

    public static IOutgoingAttachments OutgoingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Outgoing;

    public static IOutgoingAttachments OutgoingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Outgoing;
}