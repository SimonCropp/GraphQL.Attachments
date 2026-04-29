namespace GraphQL.Attachments;

public class IncomingAttachments :
    Dictionary<string, AttachmentStream>,
    IIncomingAttachments
{
    public IncomingAttachments()
    {
    }

    public IncomingAttachments(Dictionary<string, AttachmentStream> dictionary) :
        base(dictionary)
    {
    }

    public AttachmentStream GetValue()
    {
        if (Count == 0)
        {
            throw new("Reading an attachment with no name requires a single attachment to exist, but none were found.");
        }

        if (Count > 1)
        {
            throw new($"Reading an attachment with no name is only supported when there is a single attachment. Found {Count} attachments: {string.Join(", ", Keys)}.");
        }

        return Values.Single();
    }

    public AttachmentStream GetValue(string name)
    {
        if (TryGetValue(name, out var attachment))
        {
            return attachment;
        }

        if (Count == 0)
        {
            throw new($"Attachment '{name}' not found. No attachments are available.");
        }

        throw new($"Attachment '{name}' not found. Available attachments: {string.Join(", ", Keys)}.");
    }

    public bool TryGetValue([NotNullWhen(true)] out AttachmentStream? stream)
    {
        if (Count == 0)
        {
            stream = null;
            return false;
        }

        if (Count > 1)
        {
            throw new($"Reading an attachment with no name is only supported when there is a single attachment. Found {Count} attachments: {string.Join(", ", Keys)}.");
        }

        stream = Values.Single();
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var stream in Values)
        {
            await stream.DisposeAsync();
        }
    }
}