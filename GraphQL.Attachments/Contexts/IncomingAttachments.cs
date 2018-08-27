using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Attachments;

class IncomingAttachments : IIncomingAttachments, IDisposable
{
    ConcurrentDictionary<string, AttachmentStream> dictionary;

    public IncomingAttachments()
    {
        dictionary = new ConcurrentDictionary<string, AttachmentStream>(); 
    }

    public IncomingAttachments(Dictionary<string, AttachmentStream> dictionary)
    {
        Guard.AgainstNull(nameof(dictionary), dictionary);
        this.dictionary = new ConcurrentDictionary<string, AttachmentStream>(dictionary);
    }

    public AttachmentStream Read(string name)
    {
        if (TryRead(name, out var stream))
        {
            return stream;
        }
        throw new Exception($"Attachment named {name} not found.");
    }

    public bool TryRead(string name, out AttachmentStream stream)
    {
        Guard.AgainstNullWhiteSpace(nameof(name), name);
        return dictionary.TryGetValue(name, out stream);
    }

    public AttachmentStream Read()
    {
        if (TryRead(out var stream))
        {
            return stream;
        }
        throw new Exception("Attachment not found.");
    }

    public bool TryRead(out AttachmentStream stream)
    {
        if (dictionary.Count == 0)
        {
            stream = null;
            return false;
        }

        EnsureSingle();

        stream = dictionary.Single().Value;

        return true;
    }

    void EnsureSingle()
    {
        if (dictionary.Count != 1)
        {
            throw new Exception("Reading an attachment with no name is only supported when their is a single attachment.");
        }
    }

    public void Dispose()
    {
        if (dictionary != null)
        {
            foreach (var stream in dictionary.Values)
            {
                stream.Dispose();
            }
        }
    }
}