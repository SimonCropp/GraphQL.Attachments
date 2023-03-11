using System.Net.Http.Headers;

class Outgoing
{
    public Func<Cancellation, Task<HttpContent>> ContentBuilder { get; }
    public Action? Cleanup { get; }
    public HttpContentHeaders? Headers { get; }

    public Outgoing(Func<Cancellation, Task<HttpContent>> contentBuilder, Action? cleanup, HttpContentHeaders? headers)
    {
        ContentBuilder = contentBuilder;
        Cleanup = cleanup;
        Headers = headers;
    }
}