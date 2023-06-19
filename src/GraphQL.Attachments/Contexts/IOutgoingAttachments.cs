using System.Net.Http.Headers;

namespace GraphQL.Attachments;

/// <summary>
/// Provides access to write attachments.
/// </summary>
public interface IOutgoingAttachments
{
    /// <summary>
    /// Returns <code>true</code> if there are pending attachments to be written in the current outgoing pipeline.
    /// </summary>
    bool HasPendingAttachments { get; }

    /// <summary>
    /// All attachment names for the current outgoing pipeline.
    /// </summary>
    IReadOnlyList<string> Names { get; }

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream<T>(string name, Func<Cancel, Task<T>> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
        where T : Stream;

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream(string name, Func<Stream> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream(string name, Stream stream, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream<T>(Func<Cancel, Task<T>> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null)
        where T : Stream;

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream(Func<Stream> streamFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    void AddStream(Stream stream, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, Func<byte[]> byteFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, byte[] bytes, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(Func<Cancel, Task<byte[]>> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, Func<Cancel, Task<byte[]>> bytesFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(Func<byte[]> byteFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Save an attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(byte[] bytes, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string name, Func<string> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string name, string value, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(Func<Cancel, Task<string>> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string name, Func<Cancel, Task<string>> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(Func<string> valueFactory, Action? cleanup = null, HttpContentHeaders? headers = null);

    /// <summary>
    /// Save an attachment with the default name of <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string value, Action? cleanup = null, HttpContentHeaders? headers = null);
}