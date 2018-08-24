namespace GraphQL.Attachments
{
    public class AttachmentContext
    {
        public IncomingAttachments Incoming { get; }
        public OutgoingAttachments Outgoing { get; }

        public AttachmentContext()
        {
            Outgoing = new OutgoingAttachments();
            Incoming = new IncomingAttachments();
        }

        public AttachmentContext(IncomingAttachments incoming)
        {
            Guard.AgainstNull(nameof(incoming), incoming);
            Incoming = incoming;
            Outgoing = new OutgoingAttachments();
        }
    }
}