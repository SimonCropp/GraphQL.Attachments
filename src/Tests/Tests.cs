using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
[UsesVerify]
public class Tests
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
        IncomingAttachments incomingAttachments = new();
        var stream = BuildStream();
        HeaderDictionary metadata = new()
        {
            {"header1", "headerValue"}
        };
        incomingAttachments.Add("key", new("key", stream, 3, metadata));
        ServiceCollection services = new();

        TestServices.AddGraphQlTestTypes(services);

        return QueryRunner.ExecuteQuery(queryString, services, incomingAttachments);
    }

    static MemoryStream BuildStream()
    {
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write("Incoming");
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}