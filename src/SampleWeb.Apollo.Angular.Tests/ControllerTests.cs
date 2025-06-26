using Microsoft.AspNetCore.Mvc.Testing;

[TestFixture]
public class ControllerTests
{
    static QueryExecutor executor;
    static HttpClient client;

    static ControllerTests()
    {
        var factory = new WebApplicationFactory<Program>();
        client = factory.CreateClient();
        executor = new(client);
    }

    [Test]
    public Task Get_with_attachment()
    {
        var query = """
                    {
                      withAttachment (argument: "argumentValue")
                      {
                        argument
                      }
                    }
                    """;
        return Verify(executor.ExecuteGet(query));
    }

    [Test]
    public async Task Post_with_attachment_as_input()
    {
        var mutation = """
                       {
                         "operationName": "withAttachmentAsInput",
                         "variables": {"file": null},
                         "query": "mutation withAttachmentAsInput($file: Upload) { withAttachmentAsInput(argument: \"test\", file: $file) { argument __typename } }"
                       }
                       """;

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent(mutation), "operations"
            }
        };
        content.AddContent("file", "foo");
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }
}