using GraphQL;
using GraphQL.Attachments;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public class RequestReaderTests
{
    static HttpReaderWriter readerWriter = new(new GraphQLSerializer(indent: true));

    [Fact]
    public Task Parsing() =>
        Parse("{\"query\":\"{\\n  noAttachment\\n  {\\n    argument\\n  }\\n}\",\"variables\":null,\"operationName\":\"theOperation\"}");

    [Fact]
    public Task ParsingMinimal() =>
        Parse("{\"query\":\"{foo}\"}");

    [Fact]
    public Task ParsingIntrospectionQuery() =>
        Parse("{\"query\":\"\\n    query IntrospectionQuery {\\n      __schema {\\n        queryType { name }\\n        mutationType { name }\\n        subscriptionType { name }\\n        types {\\n          ...FullType\\n        }\\n        directives {\\n          name\\n          description\\n          locations\\n          args {\\n            ...InputValue\\n          }\\n        }\\n      }\\n    }\\n\\n    fragment FullType on __Type {\\n      kind\\n      name\\n      description\\n      fields(includeDeprecated: true) {\\n        name\\n        description\\n        args {\\n          ...InputValue\\n        }\\n        type {\\n          ...TypeRef\\n        }\\n        isDeprecated\\n        deprecationReason\\n      }\\n      inputFields {\\n        ...InputValue\\n      }\\n      interfaces {\\n        ...TypeRef\\n      }\\n      enumValues(includeDeprecated: true) {\\n        name\\n        description\\n        isDeprecated\\n        deprecationReason\\n      }\\n      possibleTypes {\\n        ...TypeRef\\n      }\\n    }\\n\\n    fragment InputValue on __InputValue {\\n      name\\n      description\\n      type { ...TypeRef }\\n      defaultValue\\n    }\\n\\n    fragment TypeRef on __Type {\\n      kind\\n      name\\n      ofType {\\n        kind\\n        name\\n        ofType {\\n          kind\\n          name\\n          ofType {\\n            kind\\n            name\\n            ofType {\\n              kind\\n              name\\n              ofType {\\n                kind\\n                name\\n                ofType {\\n                  kind\\n                  name\\n                  ofType {\\n                    kind\\n                    name\\n                  }\\n                }\\n              }\\n            }\\n          }\\n        }\\n      }\\n    }\\n  \",\"operationName\":\"IntrospectionQuery\"}");

    [Fact]
    public Task ParsingWithVariables() =>
        Parse("{\"query\":\"{\\n  noAttachment\\n  {\\n    argument\\n  }\\n}\",\"variables\":{\"key\":\"value\"},\"operationName\":\"theOperation\"}");

    static async Task Parse(string chars)
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chars))
        {
            Position = 0
        };
        var (query, inputs, operation) = await readerWriter.ReadBody(stream, Cancel.None);
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
                new()
                {
                    {
                        "query",
                        new("theQuery")
                    }
                },
                new FormFileCollection()),
        };
        var result = await readerWriter.ReadPost(mockHttpRequest);
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
        var result = readerWriter.ReadGet(mockHttpRequest);
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
                            new Inputs(new Dictionary<string, object?>
                            {
                                {"key", "value"}
                            })))
                    }
                })
        };
        var result = readerWriter.ReadGet(mockHttpRequest);
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
        var attachment1Bytes = "Attachment1 Text"u8.ToArray();
        var attachment2Bytes = "Attachment2 Text"u8.ToArray();
        var mockHttpRequest = new MockHttpRequest
        {
            Form = new FormCollection(
                new()
                {
                    {
                        "query",
                        new("theQuery")
                    },
                    {
                        "operation",
                        new("theOperation")
                    },
                    {
                        "variables",
                        new(JsonConvert.SerializeObject(
                            new Inputs(new Dictionary<string, object?>
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
        var result = await readerWriter.ReadPost(mockHttpRequest);
        await Verify(new
        {
            result.attachments,
            result.inputs,
            result.operation,
            result.query
        });
    }
}