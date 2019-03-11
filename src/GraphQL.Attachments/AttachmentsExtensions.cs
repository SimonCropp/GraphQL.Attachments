using System.Threading.Tasks;
using GraphQL.Attachments;

namespace GraphQL
{
    /// <summary>
    /// Extensions to GraphQL to enable Attachments.
    /// </summary>
    public static class AttachmentsExtensions
    {
        /// <summary>
        /// Executes a graphql query and makes attachments available.
        /// </summary>
        public static async Task<AttachmentExecutionResult> ExecuteWithAttachments(this IDocumentExecuter executer, ExecutionOptions options, IIncomingAttachments incomingAttachments = null)
        {
            var attachmentContext = GetAttachmentContext(incomingAttachments);

            Guard.AgainstNull(nameof(executer), executer);
            Guard.AgainstNull(nameof(options), options);
            var middleware = new AttachmentMiddleware(attachmentContext);
            options.FieldMiddleware.Use(
                next =>
                {
                    return context => middleware.Resolve(context, next);
                });
            var result = await executer.ExecuteAsync(options);
            return new AttachmentExecutionResult(result, attachmentContext.Outgoing);
        }

        static AttachmentContext GetAttachmentContext(IIncomingAttachments incoming)
        {
            if (incoming == null)
            {
                return new AttachmentContext();
            }

            return new AttachmentContext(incoming);
        }
    }
}