using System;
using System.Collections.Generic;
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
            SetContext(options, attachmentContext);
            var result = await executer.ExecuteAsync(options);
            return new AttachmentExecutionResult(result, attachmentContext.Outgoing);
        }

        static void SetContext(ExecutionOptions options, AttachmentContext attachmentContext)
        {
            if (options.UserContext == null)
            {
                options.UserContext = new Dictionary<string, object>()
                {
                    {"GraphQL.Attachments.AttachmentContext", attachmentContext}
                };
                return;
            }

            if (options.UserContext is IDictionary<string, object> dictionary)
            {
                dictionary.Add("GraphQL.Attachments.AttachmentContext", attachmentContext);
                return;
            }

            throw new Exception("Expected UserContext to be of type IDictionary<string, object>.");
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