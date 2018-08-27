using System.Collections.Generic;

namespace GraphQL.Attachments
{
    public interface IIncomingAttachments: IEnumerable<AttachmentStream>
    {
        AttachmentStream Read(string name);
        AttachmentStream Read();
        bool TryRead(out AttachmentStream func);
        bool TryRead(string name, out AttachmentStream func);
        ushort Count { get; }
    }
}