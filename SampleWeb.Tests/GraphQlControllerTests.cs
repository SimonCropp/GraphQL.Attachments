using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

public class GraphQlControllerTests
{
    HttpClient client;
    ClientQueryExecutor queryExecutor;

    public GraphQlControllerTests()
    {
        var server = GetTestServer();
        client = server.CreateClient();
        queryExecutor = new ClientQueryExecutor(client);
    }

    [Fact]
    public async Task Get()
    {
        var query = @"
{
  item
  {
    name
  }
}";
        var response = await queryExecutor.ExecuteGet(query);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"item\":{\"name\":\"TheName\"}}}", result);
    }

    [Fact]
    public async Task BodyPost()
    {
        var mutation = @"mutation ($item:ItemInput!){ addItem(item: $item) { itemCount byteCount } }";
        var variables = new
        {
            item = new
            {
                name = "TheName"
            }
        };
        var response = await queryExecutor.ExecutePost(mutation, variables);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"addItem\":{\"itemCount\":2,\"byteCount\":0}}}", result);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task MultiFormPost()
    {
        var mutation = @"mutation ($item:ItemInput!){ addItem(item: $item) { itemCount byteCount } }";
        var variables = new
        {
            item = new
            {
                name = "TheName"
            }
        };
        var response = await queryExecutor.ExecuteMultiFormPost(
            mutation,
            variables,
            attachments: new Dictionary<string, byte[]>{{"key", Encoding.UTF8.GetBytes("foo") }});
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"addItem\":{\"itemCount\":2,\"byteCount\":3}}}", result);
        response.EnsureSuccessStatusCode();
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}