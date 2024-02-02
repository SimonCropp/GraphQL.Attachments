class Outgoing(Func<Cancel, Task<HttpContent>> contentBuilder, Action? cleanup, HttpContentHeaders? headers)
{
    public Func<Cancel, Task<HttpContent>> ContentBuilder { get; } = contentBuilder;
    public Action? Cleanup { get; } = cleanup;
    public HttpContentHeaders? Headers { get; } = headers;
}