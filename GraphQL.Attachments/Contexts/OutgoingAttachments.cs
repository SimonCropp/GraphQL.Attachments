using System.Collections.Generic;

namespace GraphQL.Attachments
{
    public class OutgoingAttachments
    {
        public Dictionary<string, byte[] > dictionary  =new   Dictionary<string, byte[]>();
        public void Add(string key, byte[] bytes)
        {
            dictionary.Add(key,bytes);
        }
    }
}