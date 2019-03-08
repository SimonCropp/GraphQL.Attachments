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
        public static async Task<AttachmentExecutionResult> ExecuteWithAttachments(this IDocumentExecuter executer, ExecutionOptions options, IIncomingAttachments incoming = null)
        {
            var attachmentContext = GetAttachmentContext(incoming);

            Guard.AgainstNull(nameof(executer), executer);
            Guard.AgainstNull(nameof(options), options);
            var validationMiddleware = new AttachmentMiddleware(attachmentContext);
            options.FieldMiddleware.Use(next => { return context => validationMiddleware.Resolve(context, next); });
            var executionResult = await executer.ExecuteAsync(options);
            return new AttachmentExecutionResult(executionResult, attachmentContext.Outgoing);
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