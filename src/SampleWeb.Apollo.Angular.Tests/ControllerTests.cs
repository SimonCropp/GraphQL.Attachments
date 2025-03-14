using Microsoft.AspNetCore.Mvc.Testing;

namespace SampleWeb.Apollo.Angular.Tests;

public class ControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    readonly QueryExecutor executor;
    readonly HttpClient client;

    public ControllerTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
        executor = new(client);
    }

    [Fact]
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

    [Fact]
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