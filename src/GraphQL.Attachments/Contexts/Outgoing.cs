using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Outgoing
{
    public Func<Task<HttpContent>> ContentBuilder { get; }
    public Action? Cleanup { get; }
    public HttpContentHeaders? Headers { get; }

    public Outgoing(Func<Task<HttpContent>> contentBuilder, Action? cleanup, HttpContentHeaders? headers)
    {
        ContentBuilder = contentBuilder;
        Cleanup = cleanup;
        Headers = headers;
    }
}