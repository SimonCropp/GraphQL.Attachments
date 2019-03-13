using System.IO;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class Tests: TestBase
{
    [Fact]
    public async Task Mutation()
    {
        var mutation = @"
mutation
{
  withAttachment (argument: 'argumentValue')
  {
    argument
  }
}";
        var result = await RunQuery(mutation);
        ObjectApprover.VerifyWithJson(result);
    }

    [Fact]
    public async Task Query()
    {
        var queryString = @"
{
  withAttachment (argument: 'argumentValue')
  {
    argument
  }
}";
        var result = await RunQuery(queryString);
        ObjectApprover.VerifyWithJson(result);
    }

    static Task<AttachmentExecutionResult> RunQuery(string queryString)
    {
        var incomingAttachments = new IncomingAttachments();
        var memoryStream = BuildStream();
        var metadata = new HeaderDictionary
        {
            {"header1", "headerValue"}
        };
        incomingAttachments.Add("key", new AttachmentStream("key", memoryStream, 3, metadata));
        var services = new ServiceCollection();

        TestServices.AddGraphQlTestTypes(services);

        return QueryExecutor.ExecuteQuery(queryString, services, incomingAttachments);
    }

    static MemoryStream BuildStream()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write("Incoming");
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public Tests(ITestOutputHelper output) : base(output)
    {
    }
}