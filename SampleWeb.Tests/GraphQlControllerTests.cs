using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

public class GraphQlControllerTests
{
    static HttpClient client;

    static GraphQlControllerTests()
    {
        var server = GetTestServer();
        client = server.CreateClient();
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
        var response = await ClientQueryExecutor.ExecuteGet(client, query);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"item\":{\"name\":\"TheName\"}}}", result);
    }

    [Fact]
    public async Task Post()
    {
        var mutation = @"mutation ($item:ItemInput!){ addItem(item: $item) { count } }";
        var variables = new
        {
            item = new
            {
                name = "TheName"
            }
        };
        var response = await ClientQueryExecutor.ExecutePost(client, mutation, variables);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"addItem\":{\"count\":2}}}", result);
        response.EnsureSuccessStatusCode();
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}