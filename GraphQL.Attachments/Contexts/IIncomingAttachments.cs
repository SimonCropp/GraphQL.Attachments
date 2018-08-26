namespace GraphQL.Attachments
{
    public interface IIncomingAttachments
    {
        AttachmentStream Read(string name);
        AttachmentStream Read();
        bool TryRead(out AttachmentStream func);
        bool TryRead(string name, out AttachmentStream func);
    }
}