using System.Text;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

public class GraphQlControllerTests
{
    ClientQueryExecutor queryExecutor;

    public GraphQlControllerTests()
    {
        var server = GetTestServer();
        var client = server.CreateClient();
        queryExecutor = new ClientQueryExecutor(client);
    }

    [Fact]
    public async Task GetIntrospection()
    {
        var response = await queryExecutor.ExecuteGet(SchemaIntrospection.IntrospectionQuery);
        var result = response.ResultStream.ConvertToString();
        Assert.Contains("addItem", result);
        Assert.Contains("item", result);
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
        var result = response.ResultStream.ConvertToString();
        Assert.Equal("{\"data\":{\"item\":{\"name\":\"TheName\"}}}", result);
    }

    [Fact]
    public async Task Get_with_attachment()
    {
        var query = @"
{
  itemWithAttachment
  {
    name
  }
}";
        var response = await queryExecutor.ExecuteGet(query);
        var result = response.ResultStream.ConvertToString();
        Assert.Equal("{\"data\":{\"itemWithAttachment\":{\"name\":\"TheName\"}}}", result);
        var responseAttachment = response.Attachments["key"];
        Assert.Equal("foo", responseAttachment.Stream.ConvertToString());
    }

    [Fact]
    public async Task Post()
    {
        var mutation = "mutation ($item:ItemInput!){ addItem(item: $item) { itemCount attachmentCount } }";
        var queryRequest = new PostRequest(mutation)
        {
            Variables = new
            {
                item = new
                {
                    name = "TheName"
                }
            }
        };
        var response = await queryExecutor.ExecutePost(queryRequest );

        Assert.Equal("{\"data\":{\"addItem\":{\"itemCount\":2,\"attachmentCount\":0}}}", response.ResultStream.ConvertToString());
        Assert.Empty(response.Attachments);
    }

    [Fact]
    public async Task Post_with_attachment()
    {
        var mutation = "mutation ($item:ItemInput!){ addItem(item: $item) { itemCount attachmentCount } }";
        var postRequest = new PostRequest(mutation)
        {
            Variables = new
            {
                item = new
                {
                    name = "TheName"
                }
            },
            Action = context =>
            {
                context.AddAttachment("key", Encoding.UTF8.GetBytes("foo"));
            }
        };
        var response = await queryExecutor.ExecutePost(postRequest);
        Assert.Equal("{\"data\":{\"addItem\":{\"itemCount\":2,\"attachmentCount\":1}}}", response.ResultStream.ConvertToString());
        var responseAttachment = response.Attachments["key"];
        Assert.Equal("foo", responseAttachment.Stream.ConvertToString());
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}