using System.Collections.Generic;

namespace GraphQL.Attachments
{
    public interface IIncomingAttachments: IReadOnlyDictionary<string,AttachmentStream>
    {
        AttachmentStream GetValue();
        bool TryGetValue(out AttachmentStream func);
    }
}