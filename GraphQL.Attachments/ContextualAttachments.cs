using System;
using GraphQL.Types;

namespace GraphQL.Attachments
{
    public static class ContextualAttachments
    {
        static Func<object, IncomingAttachments> readContextFunc;

        public static void SetContextFuncs(
            Func<object, IncomingAttachments> readAttachmentFromUserContext)
        {
            Guard.AgainstNull(nameof(readAttachmentFromUserContext), readAttachmentFromUserContext);
            readContextFunc = readAttachmentFromUserContext;
        }

        public static IncomingAttachments Attachments<TSource>(this ResolveFieldContext<TSource> context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext);
        }

        public static IncomingAttachments Attachments(this ResolveFieldContext context)
        {
            Guard.AgainstNull(nameof(context), context);
            return ReadContextFunc(context.UserContext);
        }

        static IncomingAttachments ReadContextFunc(object contextUserContext)
        {
            var attachments = readContextFunc(contextUserContext);

            if (attachments == null)
            {
                return new IncomingAttachments();
            }

            return attachments;
        }
    }
}