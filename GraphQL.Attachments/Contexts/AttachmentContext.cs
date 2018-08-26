using System;

namespace GraphQL.Attachments
{
    public class AttachmentContext: IDisposable
    {
        public IIncomingAttachments Incoming { get; }
        public IOutgoingAttachments Outgoing { get; }

        public AttachmentContext()
        {
            Outgoing = new OutgoingAttachments();
            Incoming = new IncomingAttachments();
        }

        public AttachmentContext(IIncomingAttachments incoming)
        {
            Guard.AgainstNull(nameof(incoming), incoming);
            Incoming = incoming;
            Outgoing = new OutgoingAttachments();
        }

        public void Dispose()
        {
            if (Incoming is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}