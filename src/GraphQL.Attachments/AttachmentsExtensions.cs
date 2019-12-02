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
        public static async Task<AttachmentExecutionResult> ExecuteWithAttachments(this IDocumentExecuter executer, ExecutionOptions options, IIncomingAttachments? incomingAttachments = null)
        {
            var attachmentContext = BuildAttachmentContext(incomingAttachments);

            Guard.AgainstNull(nameof(executer), executer);
            Guard.AgainstNull(nameof(options), options);
            options.SetAttachmentContext(attachmentContext);
            var result = await executer.ExecuteAsync(options);
            return new AttachmentExecutionResult(result, attachmentContext.Outgoing);
        }

        static AttachmentContext BuildAttachmentContext(IIncomingAttachments? incoming)
        {
            if (incoming == null)
            {
                return new AttachmentContext();
            }

            return new AttachmentContext(incoming);
        }
    }
}