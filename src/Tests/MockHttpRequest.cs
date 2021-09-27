using Microsoft.AspNetCore.Http;

public class MockHttpRequest :
    HttpRequest
{
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Form);
    }

    public override HttpContext HttpContext { get; } = null!;
    public override string Method { get; set; } = null!;
    public override string Scheme { get; set; } = null!;
    public override bool IsHttps { get; set; }
    public override HostString Host { get; set; }
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IQueryCollection Query { get; set; } = null!;
    public override string Protocol { get; set; } = null!;
    public override IHeaderDictionary Headers { get; } = null!;
    public override IRequestCookieCollection Cookies { get; set; } = null!;
    public override long? ContentLength { get; set; }
    public override string ContentType { get; set; } = null!;
    public override Stream Body { get; set; } = null!;

    public override bool HasFormContentType
    {
        get { return Form != null; }
    }

    public override IFormCollection? Form { get; set; }
}