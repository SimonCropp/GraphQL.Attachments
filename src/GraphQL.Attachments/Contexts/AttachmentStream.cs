﻿namespace GraphQL.Attachments;

/// <summary>
/// Wraps a <see cref="Stream"/> to provide extra information when reading.
/// </summary>
public class AttachmentStream :
    Stream
{
    Stream inner;

    /// <summary>
    /// Initialises a new instance of <see cref="AttachmentStream"/>.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="inner">The <see cref="Stream"/> to wrap.</param>
    /// <param name="length">The length of <paramref name="inner"/>.</param>
    /// <param name="metadata">The attachment metadata.</param>
    public AttachmentStream(string name, Stream inner, long? length = null, IHeaderDictionary? metadata = null)
    {
        Guard.AgainstNullWhiteSpace(name);
        this.inner = inner;
        Name = name;
        Length = length ?? inner.Length;
        Metadata = metadata ?? new HeaderDictionary();
    }

    public async Task<byte[]> ToArray()
    {
        using var stream = new MemoryStream();
        await CopyToAsync(stream);
        return stream.ToArray();
    }

    public override void EndWrite(IAsyncResult asyncResult) =>
        inner.EndWrite(asyncResult);

    public override void Flush() =>
        inner.Flush();

    public override Task FlushAsync(Cancel cancel) =>
        inner.FlushAsync(cancel);

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, Cancel cancel) =>
        inner.ReadAsync(buffer, offset, count, cancel);

    public override int ReadByte() =>
        inner.ReadByte();

    public override void CopyTo(Stream destination, int bufferSize) =>
        inner.CopyTo(destination, bufferSize);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, Cancel cancel = default) =>
        inner.WriteAsync(buffer, cancel);

    public override void Write(ReadOnlySpan<byte> buffer) =>
        inner.Write(buffer);

    public override int Read(Span<byte> buffer) =>
        inner.Read(buffer);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, Cancel cancel = default) =>
        inner.ReadAsync(buffer, cancel);

    public override long Seek(long offset, SeekOrigin origin) =>
        inner.Seek(offset, origin);

    public override int Read(byte[] buffer, int offset, int count) =>
        inner.Read(buffer, offset, count);

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanTimeout => inner.CanTimeout;
    public override bool CanWrite => false;

    public string Name { get; }
    public override long Length { get; }

    /// <summary>
    /// The attachment metadata.
    /// </summary>
    public readonly IHeaderDictionary Metadata;
    public override int ReadTimeout => inner.ReadTimeout;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        inner.BeginRead(buffer, offset, count, callback, state);

    public override void Close()
    {
        inner.Close();
        base.Close();
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, Cancel cancel) =>
        inner.CopyToAsync(destination, bufferSize, cancel);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        inner.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await inner.DisposeAsync();
    }

    public override int EndRead(IAsyncResult asyncResult) =>
        inner.EndRead(asyncResult);

    public override bool Equals(object? obj) =>
        inner.Equals(obj);

    public override int GetHashCode() =>
        inner.GetHashCode();

    public override string? ToString() =>
        inner.ToString();

    public override void SetLength(long value) =>
        throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotImplementedException();

    public override Task WriteAsync(byte[] buffer, int offset, int count, Cancel cancel) =>
        throw new NotImplementedException();

    public override void WriteByte(byte value) =>
        throw new NotImplementedException();

    public override int WriteTimeout => throw new NotImplementedException();

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        throw new NotImplementedException();
}