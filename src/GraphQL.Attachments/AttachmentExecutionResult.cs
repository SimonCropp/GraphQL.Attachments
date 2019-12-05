using GraphQL.Attachments;

namespace GraphQL
{
    /// <summary>
    /// The result of a a query execution by <see cref="AttachmentsExtensions.ExecuteWithAttachments"/>.
    /// </summary>
    public class AttachmentExecutionResult
    {
        public ExecutionResult ExecutionResult { get; }
        public IOutgoingAttachments Attachments { get; }

        public AttachmentExecutionResult(ExecutionResult executionResult, IOutgoingAttachments attachments)
        {
            Guard.AgainstNull(nameof(executionResult), executionResult);
            Guard.AgainstNull(nameof(attachments), attachments);
            ExecutionResult = executionResult;
            Attachments = attachments;
        }
    }
}