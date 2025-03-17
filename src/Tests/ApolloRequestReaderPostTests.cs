using Newtonsoft.Json;

[TestFixture]
public class ApolloRequestReaderPostTests
{
    [Test]
    public async Task ReadPost()
    {
        var attachment1Bytes = "Attachment1 Text"u8.ToArray();
        var mockHttpRequest = new MockHttpRequest
        {
            Form = new FormCollection(
                new()
                {
                    {
                        "operations", new(JsonConvert.SerializeObject(
                            new ApolloPostRequestReader.ApolloPostRequestOperation
                            {
                                operationName = "TestOperation",
                                query = "The query",
                                variables = JsonConvert.SerializeObject(
                                    new Inputs(new Dictionary<string, object?>
                                    {
                                        {
                                            "key", "value"
                                        }
                                    }))
                            }
                        ))
                    }
                },
                new FormFileCollection
                {
                    new FormFile(new MemoryStream(attachment1Bytes), 0, attachment1Bytes.Length, "attachment1", "attachment1.txt")
                    {
                        Headers = new HeaderDictionary
                        {
                            {
                                "file1Header", "file1HeaderValue"
                            }
                        }
                    }
                }),
        };
        var result = await ApolloPostRequestReader.ReadPost(mockHttpRequest);
        await Verify(new
        {
            result.attachments,
            result.inputs,
            result.operation,
            result.query
        });
    }
}