using System.Net.Http;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class GraphQlControllerTests:
    VerifyBase
{
    static ClientQueryExecutor queryExecutor;
    static TestServer server;
    static HttpClient client;

    static GraphQlControllerTests()
    {
        server = GetTestServer();
        client = server.CreateClient();
        queryExecutor = new ClientQueryExecutor(client);
    }
    public GraphQlControllerTests(ITestOutputHelper output) :
        base(output)
    {
    }

    [Fact]
    public async Task GetIntrospection()
    {
        var result = await queryExecutor.ExecuteGet(SchemaIntrospection.IntrospectionQuery);
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
        var result = await queryExecutor.ExecuteGet(query);
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
        var result = await queryExecutor.ExecuteGet(query);
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
        var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        var response = await client.PostAsync("graphql", content);
        var queryResult = await response.ProcessResponse();
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

        var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        content.AddContent("key", "foo");
        var response = await client.PostAsync("graphql", content);
        var queryResult = await response.ProcessResponse();
        await Verify(queryResult);
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}