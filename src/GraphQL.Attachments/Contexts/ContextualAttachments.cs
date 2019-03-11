using GraphQL.Types;

namespace GraphQL.Attachments
{
    public static class ContextualAttachments
    {
        public static IIncomingAttachments IncomingAttachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return context.GetAttachmentContext().Incoming;
        }

        public static IOutgoingAttachments OutgoingAttachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return context.GetAttachmentContext().Outgoing;
        }

        public static IOutgoingAttachments OutgoingAttachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return context.GetAttachmentContext().Outgoing;
        }

        public static IIncomingAttachments IncomingAttachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return context.GetAttachmentContext().Incoming;
        }
    }
}