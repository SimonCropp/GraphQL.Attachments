using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphQL.Attachments
{
    public class IncomingAttachments
    {
        ConcurrentDictionary<string, Func<Stream>> dictionary;
        List<string> names;

        public IncomingAttachments()
        {
            names = new List<string>();
        }

        public IncomingAttachments(Dictionary<string, Func<Stream>> dictionary)
        {
            Guard.AgainstNull(nameof(dictionary), dictionary);
            names = dictionary.Keys.ToList();
            this.dictionary = new ConcurrentDictionary<string, Func<Stream>>(dictionary);
        }

        public bool TryRead(string name, out Stream stream)
        {
            Guard.AgainstNullWhiteSpace(nameof(name), name);
            if (!names.Contains(name))
            {
                stream = null;
                return false;
            }

            GetAndRemove(name, out var value);

            stream = value();
            return true;
        }

        public bool TryRead(out Func<Stream> func)
        {
            if (names.Count == 0)
            {
                func = null;
                return false;
            }

            EnsureSingle();

            var name = names.Single();

            GetAndRemove(name, out func);

            return true;
        }

        void GetAndRemove(string name, out Func<Stream> func)
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

        void EnsureNameExists(string name)
        {
            if (!names.Contains(name))
            {
                throw new Exception($"Could not find an attachment named '{name}'.");
            }
        }
    }
}