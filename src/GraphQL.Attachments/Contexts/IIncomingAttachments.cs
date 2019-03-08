using System;
using System.Collections.Generic;

namespace GraphQL.Attachments
{
    public interface IIncomingAttachments:
        IReadOnlyDictionary<string, AttachmentStream>,
        IDisposable
    {
        AttachmentStream GetValue();
        bool TryGetValue(out AttachmentStream func);
    }
}