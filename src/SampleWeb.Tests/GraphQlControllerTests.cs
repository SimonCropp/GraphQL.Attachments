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
    static QueryExecutor queryExecutor;
    static TestServer server;
    static HttpClient client;

    static GraphQlControllerTests()
    {
        server = GetTestServer();
        client = server.CreateClient();
        queryExecutor = new QueryExecutor(client);
    }

    public GraphQlControllerTests(ITestOutputHelper output) :
        base(output)
    {
    }

    [Fact]
    public async Task GetIntrospection()
    {
        await using var result = await queryExecutor.ExecuteGet(SchemaIntrospection.IntrospectionQuery);
        await Verify(result);
    }

    [Fact]
    public async Task Get()
    {
        var query = @"
{
  noAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";
        await using var result = await queryExecutor.ExecuteGet(query);
        await Verify(result);
    }

    [Fact]
    public async Task Get_with_attachment()
    {
        var query = @"
{
  withAttachment (argument: ""argumentValue"")
  {
    argument
  }
}";
        await using var result = await queryExecutor.ExecuteGet(query);
        await Verify(result);
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
        await using var queryResult = await response.ProcessResponse();
        await Verify(queryResult);
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
        await using var queryResult = await response.ProcessResponse();
        await Verify(queryResult);
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
        await using var queryResult = await response.ProcessResponse();
        await Verify(queryResult);
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}