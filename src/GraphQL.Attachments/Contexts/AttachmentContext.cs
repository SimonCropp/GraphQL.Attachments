namespace GraphQL.Attachments;

public class AttachmentContext :
    IAsyncDisposable
{
    /// <summary>
    /// The <see cref="IIncomingAttachments"/> to pass into the query execution. Retrieved from <see cref="HttpReaderWriter.ReadGet"/> or <see cref="HttpReaderWriter.ReadPost"/>.
    /// </summary>
    public IIncomingAttachments Incoming { get; }

    /// <summary>
    /// The resulting <see cref="IOutgoingAttachments"/> that will be appended to the outgoing response via <see cref="HttpReaderWriter.WriteResult"/>.
    /// </summary>
    public IOutgoingAttachments Outgoing { get; }

    public AttachmentContext()
    {
        Outgoing = new OutgoingAttachments();
        Incoming = new IncomingAttachments();
    }

    public AttachmentContext(IIncomingAttachments incoming)
    {
        Incoming = incoming;
        Outgoing = new OutgoingAttachments();
    }

    /// <summary>
    /// <see cref="IAsyncDisposable.DisposeAsync"/>.
    /// </summary>
    public ValueTask DisposeAsync() =>
        Incoming.DisposeAsync();
}