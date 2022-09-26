using GraphQL.Attachments;

namespace GraphQL;

/// <summary>
/// Extensions to GraphQL to enable Attachments.
/// </summary>
public static class AttachmentsExtensions
{
    /// <summary>
    /// Executes a GraphQL query and makes attachments available.
    /// </summary>
    public static async Task<AttachmentExecutionResult> ExecuteWithAttachments(
        this IDocumentExecuter executer,
        ExecutionOptions options,
        IIncomingAttachments? attachments = null)
    {
        await using var attachmentContext = BuildAttachmentContext(attachments);
        options.SetAttachmentContext(attachmentContext);
        var result = await executer.ExecuteAsync(options);
        return new(result, attachmentContext.Outgoing);
    }

    public static void AddAttachmentContext(this ExecutionOptions options, AttachmentContext attachmentContext) =>
        options.SetAttachmentContext(attachmentContext);

    static AttachmentContext BuildAttachmentContext(IIncomingAttachments? incoming)
    {
        if (incoming == null)
        {
            return new();
        }

        return new(incoming);
    }
}