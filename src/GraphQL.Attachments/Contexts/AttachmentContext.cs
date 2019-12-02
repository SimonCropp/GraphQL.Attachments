using System;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public class AttachmentContext :
        IAsyncDisposable
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

        public ValueTask DisposeAsync()
        {
            return Incoming.DisposeAsync();
        }
    }
}