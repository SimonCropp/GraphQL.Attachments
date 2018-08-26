﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{

    class OutgoingAttachments : IOutgoingAttachments
    {
        internal Dictionary<string, Outgoing> Inner = new Dictionary<string, Outgoing>(StringComparer.OrdinalIgnoreCase);

        public bool HasPendingAttachments => Inner.Any();

        public IReadOnlyList<string> Names => Inner.Keys.ToList();

        public void AddStream<T>(Func<Task<T>> streamFactory, Action cleanup = null, HttpContentHeaders headers = null)
            where T : Stream
        {
            AddStream("default", streamFactory,  cleanup, headers);
        }

        public void AddStream<T>(string name, Func<Task<T>> streamFactory, Action cleanup = null, HttpContentHeaders headers = null)
            where T : Stream
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(streamFactory), streamFactory);
            Inner.Add(name, new Outgoing
            {
                AsyncStreamFactory = streamFactory.WrapStreamFuncTaskInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddStream(Func<Stream> streamFactory,  Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddStream("default", streamFactory, cleanup, headers);
        }

        public void AddStream(Stream stream, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddStream("default", stream, cleanup, headers);
        }

        public void AddStream(string name, Func<Stream> streamFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(streamFactory), streamFactory);
            Inner.Add(name, new Outgoing
            {
                StreamFactory = streamFactory.WrapFuncInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddStream(string name, Stream stream, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(stream), stream);
            Inner.Add(name, new Outgoing
            {
                StreamInstance = stream,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddBytes(Func<byte[]> bytesFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddBytes("default", bytesFactory, cleanup, headers);
        }

        public void AddBytes(byte[] bytes, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddBytes("default", bytes, cleanup, headers);
        }

        public void AddBytes(string name, Func<byte[]> bytesFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(bytesFactory), bytesFactory);
            Inner.Add(name, new Outgoing
            {
                BytesFactory = bytesFactory.WrapFuncInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddBytes(string name, byte[] bytes, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(bytes), bytes);
            Inner.Add(name, new Outgoing
            {
                BytesInstance = bytes,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddBytes(Func<Task<byte[]>> bytesFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddBytes("default", bytesFactory, cleanup, headers);
        }

        public void AddBytes(string name, Func<Task<byte[]>> bytesFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name),name);
            Guard.AgainstNull(nameof(bytesFactory), bytesFactory);
            Inner.Add(name, new Outgoing
            {
                AsyncBytesFactory = bytesFactory.WrapFuncTaskInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddString(Func<string> valueFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddString("default", valueFactory, cleanup, headers);
        }

        public void AddString(string value, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddString("default", value, cleanup, headers);
        }

        public void AddString(string name, Func<string> valueFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name), name);
            Guard.AgainstNull(nameof(valueFactory), valueFactory);
            Inner.Add(name, new Outgoing
            {
                StringFactory = valueFactory.WrapFuncInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddString(string name, string value, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name), name);
            Guard.AgainstNull(nameof(value), value);
            Inner.Add(name, new Outgoing
            {
                StringInstance = value,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }

        public void AddString(Func<Task<string>> valueFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            AddString("default", valueFactory, cleanup, headers);
        }

        public void AddString(string name, Func<Task<string>> valueFactory, Action cleanup = null, HttpContentHeaders headers = null)
        {
            Guard.AgainstNull(nameof(name), name);
            Guard.AgainstNull(nameof(valueFactory), valueFactory);
            Inner.Add(name, new Outgoing
            {
                AsyncStringFactory = valueFactory.WrapFuncTaskInCheck(name),
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Headers = headers
            });
        }
    }
}