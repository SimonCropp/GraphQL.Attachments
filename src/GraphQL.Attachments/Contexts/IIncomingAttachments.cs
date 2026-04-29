namespace GraphQL.Attachments;

public interface IIncomingAttachments:
    IReadOnlyDictionary<string, AttachmentStream>,
    IAsyncDisposable
{
    AttachmentStream GetValue();
    AttachmentStream GetValue(string name);
    bool TryGetValue([NotNullWhen(true)] out AttachmentStream? func);
}