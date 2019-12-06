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
                            {"file1Header","file1HeaderValue"}
                        }
                    },
                    new FormFile(new MemoryStream(attachment2Bytes), 0, attachment2Bytes.Length, "attachment2", "attachment2.txt")
                    {
                        Headers = new HeaderDictionary
                        {
                            {"file2Header","file2HeaderValue"}
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

    static MemoryStream BuildBody(RequestReader.PostBody body)
    {
        var serializeObject = JsonConvert.SerializeObject(body);
        return BuildStream(serializeObject);
    }

    static MemoryStream BuildStream(string value)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public RequestReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}