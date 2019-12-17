using System.IO;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
{
    [Fact]
    public Task Mutation()
    {
        var mutation = @"
mutation
{
  withAttachment (argument: 'argumentValue')
  {
    argument
  }
}";
        return Verify(RunQuery(mutation));
    }

    [Fact]
    public Task Query()
    {
        var queryString = @"
{
  withAttachment (argument: 'argumentValue')
  {
    argument
  }
}";
        return Verify(RunQuery(queryString));
    }

    static Task<AttachmentExecutionResult> RunQuery(string queryString)
    {
        var incomingAttachments = new IncomingAttachments();
        var stream = BuildStream();
        var metadata = new HeaderDictionary
        {
            {"header1", "headerValue"}
        };
        incomingAttachments.Add("key", new AttachmentStream("key", stream, 3, metadata));
        var services = new ServiceCollection();

        TestServices.AddGraphQlTestTypes(services);

        return QueryRunner.ExecuteQuery(queryString, services, incomingAttachments);
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

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}