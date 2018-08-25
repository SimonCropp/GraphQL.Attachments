using System;
using GraphQL.Types;

namespace GraphQL.Attachments
{
    public static class ContextualAttachments
    {
        static Func<object, AttachmentContext> contextFunc;

        public static void SetContextFunc(Func<object, AttachmentContext> readAttachmentsFromUserContext)
        {
            Guard.AgainstNull(nameof(readAttachmentsFromUserContext), readAttachmentsFromUserContext);
            contextFunc = readAttachmentsFromUserContext;
        }

        public static IncomingAttachments IncomingAttachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext).Incoming;
        }
        public static OutgoingAttachments OutgoingAttachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext).Outgoing;
        }

        public static OutgoingAttachments OutgoingAttachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext).Outgoing;
        }

        public static IncomingAttachments IncomingAttachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext).Incoming;
        }

        static AttachmentContext ReadContextFunc(object userContext)
        {
            if (contextFunc == null)
            {
                throw new Exception("The Func to read attachment context from the GraphQL UserContext has not been set. Once at startup ContextualAttachments.SetContextFunc should be called.");
            }

            var attachments = contextFunc(userContext);

            if (attachments == null)
            {
                throw new Exception("Could not resolve an instance of AttachmentContext.");
            }

            return attachments;
        }
    }
}