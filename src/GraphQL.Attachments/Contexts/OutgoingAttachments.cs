using System.Net.Http;
using System.Net.Http.Headers;
using GraphQL.Attachments;

class OutgoingAttachments :
    IOutgoingAttachments
{
    internal Dictionary<string, Outgoing> Inner = new(StringComparer.OrdinalIgnoreCase);

    public bool HasPendingAttachments => Inner.Any();

    public IReadOnlyList<string> Names => Inner.Keys.ToList();

    public void AddStream<T>(Func<CancellationToken, Task<T>> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
        where T : Stream
    {
        AddStream("default", streamFactory, cleanup, headers);
    }

    public void AddStream<T>(string name, Func<CancellationToken, Task<T>> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
        where T : Stream
    {
        Inner.Add(name,
            new
            (
                contentBuilder: async cancellation =>
                {
                    streamFactory = streamFactory.WrapFuncTaskInCheck(name);
                    var value = await streamFactory(cancellation);
                    return new StreamContent(value);
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddStream(Func<Stream> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddStream("default", streamFactory, cleanup, headers);
    }

    public void AddStream(Stream stream, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddStream("default", stream, cleanup, headers);
    }

    public void AddStream(string name, Func<Stream> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ =>
                {
                    streamFactory = streamFactory.WrapFuncInCheck(name);
                    var value = streamFactory();
                    return Task.FromResult<HttpContent>(new StreamContent(value));
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddStream(string name, Stream stream, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ => Task.FromResult<HttpContent>(new StreamContent(stream)),
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddBytes(Func<byte[]> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddBytes("default", bytesFactory, cleanup, headers);
    }

    public void AddBytes(byte[] bytes, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddBytes("default", bytes, cleanup, headers);
    }

    public void AddBytes(string name, Func<byte[]> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ =>
                {
                    bytesFactory = bytesFactory.WrapFuncInCheck(name);
                    var value = bytesFactory();
                    return Task.FromResult<HttpContent>(new ByteArrayContent(value));
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddBytes(string name, byte[] bytes, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ => Task.FromResult<HttpContent>(new ByteArrayContent(bytes)),
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddBytes(Func<CancellationToken, Task<byte[]>> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddBytes("default", bytesFactory, cleanup, headers);
    }

    public void AddBytes(string name, Func<CancellationToken, Task<byte[]>> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: async cancellation =>
                {
                    bytesFactory = bytesFactory.WrapFuncTaskInCheck(name);
                    var value = await bytesFactory(cancellation);
                    return new ByteArrayContent(value);
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddString(Func<string> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddString("default", valueFactory, cleanup, headers);
    }

    public void AddString(string value, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddString("default", value, cleanup, headers);
    }

    public void AddString(string name, Func<string> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ =>
                {
                    valueFactory = valueFactory.WrapFuncInCheck(name);
                    var value = valueFactory();
                    return Task.FromResult<HttpContent>(new StringContent(value));
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddString(string name, string value, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: _ => Task.FromResult<HttpContent>(new StringContent(value)),
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }

    public void AddString(Func<CancellationToken, Task<string>> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        AddString("default", valueFactory, cleanup, headers);
    }

    public void AddString(string name, Func<CancellationToken, Task<string>> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
    {
        Inner.Add(name,
            new
            (
                contentBuilder: async cancellation =>
                {
                    valueFactory = valueFactory.WrapFuncTaskInCheck(name);
                    var value = await valueFactory(cancellation);
                    return new StringContent(value);
                },
                cleanup: cleanup.WrapCleanupInCheck(name),
                headers: headers
            ));
    }
}