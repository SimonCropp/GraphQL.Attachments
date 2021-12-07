using System.Net.Http.Headers;

class Outgoing
{
    public Func<CancellationToken, Task<HttpContent>> ContentBuilder { get; }
    public Action? Cleanup { get; }
    public HttpContentHeaders? Headers { get; }

    public Outgoing(Func<CancellationToken, Task<HttpContent>> contentBuilder, Action? cleanup, HttpContentHeaders? headers)
    {
        ContentBuilder = contentBuilder;
        Cleanup = cleanup;
        Headers = headers;
    }
}