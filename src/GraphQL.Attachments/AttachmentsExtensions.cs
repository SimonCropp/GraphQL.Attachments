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
        await using var context = BuildAttachmentContext(attachments);
        options.SetAttachmentContext(context);
        var result = await executer.ExecuteAsync(options);
        return new(result, context.Outgoing);
    }

    public static void AddAttachmentContext(this ExecutionOptions options, AttachmentContext context) =>
        options.SetAttachmentContext(context);

    static AttachmentContext BuildAttachmentContext(IIncomingAttachments? incoming)
    {
        if (incoming == null)
        {
            return new();
        }

        return new(incoming);
    }
}
