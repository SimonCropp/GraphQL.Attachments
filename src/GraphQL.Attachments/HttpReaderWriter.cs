namespace GraphQL.Attachments;

/// <summary>
/// Handles parsing a <see cref="HttpRequest"/> into the corresponding query, <see cref="Inputs"/>, operation, and <see cref="IIncomingAttachments"/>.
/// Handles writing a <see cref="AttachmentExecutionResult"/> to a <see cref="HttpResponse"/>.
/// </summary>
public partial class HttpReaderWriter
{
    IGraphQLTextSerializer serializer;

    public HttpReaderWriter(IGraphQLTextSerializer serializer) =>
        this.serializer = serializer;
}