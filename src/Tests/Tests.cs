﻿[TestFixture]
public class Tests
{
    [Test]
    public Task Mutation()
    {
        var mutation = """
                       mutation
                       {
                         withAttachment (argument: 'argumentValue')
                         {
                           argument
                         }
                       }
                       """;
        return Verify(RunQuery(mutation));
    }

    [Test]
    public Task Query()
    {
        var queryString = """
                          {
                            withAttachment (argument: 'argumentValue')
                            {
                              argument
                            }
                          }
                          """;

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
        incomingAttachments.Add("key", new("key", stream, 3, metadata));
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
}