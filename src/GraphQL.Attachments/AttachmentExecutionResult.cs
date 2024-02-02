namespace GraphQL;

/// <summary>
/// The result of a query execution by <see cref="AttachmentsExtensions.ExecuteWithAttachments"/>.
/// </summary>
public class AttachmentExecutionResult(ExecutionResult executionResult, IOutgoingAttachments attachments)
{
    public ExecutionResult ExecutionResult { get; } = executionResult;
    public IOutgoingAttachments Attachments { get; } = attachments;
}