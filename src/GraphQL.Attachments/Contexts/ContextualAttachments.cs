namespace GraphQL.Attachments;

public static class ContextualAttachments
{
    public static IIncomingAttachments IncomingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment<TSource>(this IResolveFieldContext<TSource> context) =>
        context.IncomingAttachments().GetValue();

    public static AttachmentStream IncomingAttachment<TSource>(this IResolveFieldContext<TSource> context, string name) =>
        context.IncomingAttachments().GetValue(name);

    public static IIncomingAttachments IncomingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Incoming;

    public static AttachmentStream IncomingAttachment(this IResolveFieldContext context) =>
        context.IncomingAttachments().GetValue();

    public static AttachmentStream IncomingAttachment(this IResolveFieldContext context, string name) =>
        context.IncomingAttachments().GetValue(name);

    public static IOutgoingAttachments OutgoingAttachments<TSource>(this IResolveFieldContext<TSource> context) =>
        context.GetAttachmentContext().Outgoing;

    public static IOutgoingAttachments OutgoingAttachments(this IResolveFieldContext context) =>
        context.GetAttachmentContext().Outgoing;
}