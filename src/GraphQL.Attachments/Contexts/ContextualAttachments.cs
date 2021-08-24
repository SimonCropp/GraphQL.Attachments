namespace GraphQL.Attachments
{
    public static class ContextualAttachments
    {
        public static IIncomingAttachments IncomingAttachments<TSource>(this IResolveFieldContext<TSource> context)
        {
            return context.GetAttachmentContext().Incoming;
        }

        public static IOutgoingAttachments OutgoingAttachments<TSource>(this IResolveFieldContext<TSource> context)
        {
            return context.GetAttachmentContext().Outgoing;
        }

        public static IOutgoingAttachments OutgoingAttachments(this IResolveFieldContext context)
        {
            return context.GetAttachmentContext().Outgoing;
        }

        public static IIncomingAttachments IncomingAttachments(this IResolveFieldContext context)
        {
            return context.GetAttachmentContext().Incoming;
        }
    }
}