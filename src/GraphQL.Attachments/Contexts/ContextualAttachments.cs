namespace GraphQL.Attachments;

public static class ContextualAttachments
{
    public static IIncomingAttachments IncomingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment<TSource>(this IResolveFieldContext<TSource> context) =>
        context.IncomingAttachments().Single().Value;

    public static IIncomingAttachments IncomingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment(this IResolveFieldContext context) =>
        context.IncomingAttachments().Single().Value;

    public static IOutgoingAttachments OutgoingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Outgoing;

    public static IOutgoingAttachments OutgoingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Outgoing;
}