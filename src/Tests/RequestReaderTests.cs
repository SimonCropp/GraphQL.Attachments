using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class RequestReaderTests :
    VerifyBase
{
    [Fact]
    public async Task Parsing()
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"query\":\"{\\n  noAttachment\\n  {\\n    argument\\n  }\\n}\",\"variables\":null,\"operationName\":\"theOperation\"}")) {Position = 0};
        var (query, inputs, operation) = await RequestReader.ReadBody(stream);
        await Verify(new
        {
            query, inputs, operation
        });
    }

    [Fact]
    public async Task ReadPostMinimal()
    {
        var mockHttpRequest = new MockHttpRequest
        {
            Form = new FormCollection(
                new Dictionary<string, StringValues>
                {
                    {
                        "query",
                        new StringValues("theQuery")
                    }
                },
                new FormFileCollection()),
        };
        var result = await RequestReader.ReadPost(mockHttpRequest);
        await Verify(new
        {
            result.attachments,
            result.inputs,
            result.operation,
            result.query
        });
    }

    [Fact]
    public async Task ReadGetMinimal()
    {
        var mockHttpRequest = new MockHttpRequest
        {
            Query = new QueryCollection(
                new Dictionary<string, StringValues>
                {
                    {
                        "query",
                        new StringValues("theQuery")
                    }
                })
        };
        var result = RequestReader.ReadGet(mockHttpRequest);
        await Verify(new
        {
            result.inputs,
            result.operation,
            result.query
        });
    }

    [Fact]
    public async Task ReadGet()
    {
        var mockHttpRequest = new MockHttpRequest
        {
            Query = new QueryCollection(
                new Dictionary<string, StringValues>
                {
                    {
                        "query",
                        new StringValues("theQuery")
                    },
                    {
                        "operation",
                        new StringValues("theOperation")
                    },
                    {
                        "variables",
                        new StringValues(JsonConvert.SerializeObject(
                            new Inputs(new Dictionary<string, object>
                            {
                                {"key", "value"}
                            })))
                    }
                })
        };
        var result = RequestReader.ReadGet(mockHttpRequest);
        await Verify(new
        {
            result.inputs,
            result.operation,
            result.query
        });
    }

    [Fact]
    public async Task ReadPost()
    {
        var attachment1Bytes = Encoding.UTF8.GetBytes("Attachment1 Text");
        var attachment2Bytes = Encoding.UTF8.GetBytes("Attachment2 Text");
        var mockHttpRequest = new MockHttpRequest
        {
            Form = new FormCollection(
                new Dictionary<string, StringValues>
                {
                    {
                        "query",
                        new StringValues("theQuery")
                    },
                    {
                        "operation",
                        new StringValues("theOperation")
                    },
                    {
                        "variables",
                        new StringValues(JsonConvert.SerializeObject(
                            new Inputs(new Dictionary<string, object>
                            {
                                {"key", "value"}
                            })))
                    }
                },
                new FormFileCollection
                {
                    new FormFile(new MemoryStream(attachment1Bytes), 0, attachment1Bytes.Length, "attachment1", "attachment1.txt")
                    {
                        Headers = new HeaderDictionary
                        {
                            {"file1Header", "file1HeaderValue"}
                        }
                    },
                    new FormFile(new MemoryStream(attachment2Bytes), 0, attachment2Bytes.Length, "attachment2", "attachment2.txt")
                    {
                        Headers = new HeaderDictionary
                        {
                            {"file2Header", "file2HeaderValue"}
                        }
                    },
                }),
        };
        var result = await RequestReader.ReadPost(mockHttpRequest);
        await Verify(new
        {
            result.attachments,
            result.inputs,
            result.operation,
            result.query
        });
    }

    public RequestReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}