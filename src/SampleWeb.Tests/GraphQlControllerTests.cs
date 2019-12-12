using System.Net.Http;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class GraphQlControllerTests :
    VerifyBase
{
    static QueryExecutor executor;
    static TestServer server;
    static HttpClient client;

    static GraphQlControllerTests()
    {
        server = GetTestServer();
        client = server.CreateClient();
        executor = new QueryExecutor(client);
    }

    public GraphQlControllerTests(ITestOutputHelper output) :
        base(output)
    {
    }

    [Fact]
    public Task GetIntrospection()
    {
        return Verify(executor.ExecuteGet(SchemaIntrospection.IntrospectionQuery));
    }

    [Fact]
    public Task Get()
    {
        var query = @"
{
  noAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";
        return Verify(executor.ExecuteGet(query));
    }

    [Fact]
    public Task Get_with_attachment()
    {
        var query = @"
{
  withAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";
        return Verify(executor.ExecuteGet(query));
    }

    [Fact]
    public async Task Post()
    {
        var mutation = @"mutation
{
  withAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";
        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    [Fact]
    public async Task Post_with_attachment()
    {
        var mutation = @"mutation
{
  withAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";

        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        content.AddContent("key", "foo");
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    [Fact]
    public async Task Post_with_multiple_attachment()
    {
        var mutation = @"mutation
{
  withAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";

        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        content.AddContent("key1", "foo1");
        content.AddContent("key2", "foo2");
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}