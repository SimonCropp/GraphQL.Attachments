using System.Text;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class GraphQlControllerTests:
    TestBase
{
    ClientQueryExecutor queryExecutor;

    public GraphQlControllerTests(ITestOutputHelper output) :
        base(output)
    {
        var server = GetTestServer();
        var client = server.CreateClient();
        queryExecutor = new ClientQueryExecutor(client);
    }

    [Fact]
    public async Task GetIntrospection()
    {
        var result = await queryExecutor.ExecuteGet(SchemaIntrospection.IntrospectionQuery);
        ObjectApprover.VerifyWithJson(result);
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
        ObjectApprover.VerifyWithJson(result);
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
        ObjectApprover.VerifyWithJson(result);
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
        var queryRequest = new PostRequest(mutation);
        var result = await queryExecutor.ExecutePost(queryRequest );

        ObjectApprover.VerifyWithJson(result);
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
        var postRequest = new PostRequest(mutation)
        {
            Action = context =>
            {
                context.AddAttachment("key", Encoding.UTF8.GetBytes("foo"));
            }
        };
        var result = await queryExecutor.ExecutePost(postRequest);
        ObjectApprover.VerifyWithJson(result);
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}