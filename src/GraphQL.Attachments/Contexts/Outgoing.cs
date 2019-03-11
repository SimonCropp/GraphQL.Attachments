using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class Outgoing
{
    public Func<Task<HttpContent>> ContentBuilder;
    public Action Cleanup;
    public HttpContentHeaders Headers;
}