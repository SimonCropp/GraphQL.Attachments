using GraphQL.Attachments;

namespace GraphQL
{
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