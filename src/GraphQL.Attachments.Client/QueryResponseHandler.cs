using System;
using System.IO;

namespace GraphQL.Attachments
{
    public class QueryResponseHandler
    {
        public QueryResponseHandler(Action<Stream> resultAction)
        {
            Guard.AgainstNull(nameof(resultAction), resultAction);
            ResultAction = resultAction;
        }

        public Action<Attachment> AttachmentAction { get; set; }

        public Action<Stream> ResultAction { get; }
    }
}