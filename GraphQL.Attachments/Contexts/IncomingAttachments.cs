using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Attachments;

class IncomingAttachments : IIncomingAttachments, IDisposable
{
    ConcurrentDictionary<string, AttachmentStream> dictionary;
    List<string> names;

    public IncomingAttachments()
    {
        names = new List<string>();
    }

    public IncomingAttachments(Dictionary<string, AttachmentStream> dictionary)
    {
        Guard.AgainstNull(nameof(dictionary), dictionary);
        names = dictionary.Keys.ToList();
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
        if (!names.Contains(name))
        {
            stream = null;
            return false;
        }

        GetAndRemove(name, out stream);

        return true;
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
        if (names.Count == 0)
        {
            stream = null;
            return false;
        }

        EnsureSingle();

        var name = names.Single();

        GetAndRemove(name, out stream);

        return true;
    }

    void GetAndRemove(string name, out AttachmentStream func)
    {
        if (!dictionary.TryRemove(name, out func))
        {
            throw new Exception($"Found attachment named '{name}' but it has already been consumed.");
        }
    }

    void EnsureSingle()
    {
        if (names.Count != 1)
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