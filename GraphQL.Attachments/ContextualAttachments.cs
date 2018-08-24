using System;
using GraphQL.Types;

namespace GraphQL.Attachments
{
    public static class ContextualAttachments
    {
        static Func<object, AttachmentContext> contextFunc;

        public static void SetContextFunc(
            Func<object, AttachmentContext> readAttachmentsFromUserContext)
        {
            Guard.AgainstNull(nameof(readAttachmentsFromUserContext), readAttachmentsFromUserContext);
            contextFunc = readAttachmentsFromUserContext;
        }

        public static AttachmentContext Attachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext);
        }

        public static AttachmentContext Attachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext);
        }

        static AttachmentContext ReadContextFunc(object contextUserContext)
        {
            var attachments = contextFunc(contextUserContext);

            if (attachments == null)
            {
                throw new Exception("Could not resolve an instance of AttachmentContext.");
            }

            return attachments;
        }
    }

    public class OutgoingAttachments
    {
    }
}