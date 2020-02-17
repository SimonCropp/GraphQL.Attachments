﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
    public Task Parsing()
    {
        return Parse("{\"query\":\"{\\n  noAttachment\\n  {\\n    argument\\n  }\\n}\",\"variables\":null,\"operationName\":\"theOperation\"}");
    }

    [Fact]
    public Task ParsingMinimal()
    {
        return Parse("{\"query\":\"{foo}\"}");
    }

    [Fact]
    public Task ParsingIntrospectionQuery()
    {
        return Parse("{\"query\":\"\\n    query IntrospectionQuery {\\n      __schema {\\n        queryType { name }\\n        mutationType { name }\\n        subscriptionType { name }\\n        types {\\n          ...FullType\\n        }\\n        directives {\\n          name\\n          description\\n          locations\\n          args {\\n            ...InputValue\\n          }\\n        }\\n      }\\n    }\\n\\n    fragment FullType on __Type {\\n      kind\\n      name\\n      description\\n      fields(includeDeprecated: true) {\\n        name\\n        description\\n        args {\\n          ...InputValue\\n        }\\n        type {\\n          ...TypeRef\\n        }\\n        isDeprecated\\n        deprecationReason\\n      }\\n      inputFields {\\n        ...InputValue\\n      }\\n      interfaces {\\n        ...TypeRef\\n      }\\n      enumValues(includeDeprecated: true) {\\n        name\\n        description\\n        isDeprecated\\n        deprecationReason\\n      }\\n      possibleTypes {\\n        ...TypeRef\\n      }\\n    }\\n\\n    fragment InputValue on __InputValue {\\n      name\\n      description\\n      type { ...TypeRef }\\n      defaultValue\\n    }\\n\\n    fragment TypeRef on __Type {\\n      kind\\n      name\\n      ofType {\\n        kind\\n        name\\n        ofType {\\n          kind\\n          name\\n          ofType {\\n            kind\\n            name\\n            ofType {\\n              kind\\n              name\\n              ofType {\\n                kind\\n                name\\n                ofType {\\n                  kind\\n                  name\\n                  ofType {\\n                    kind\\n                    name\\n                  }\\n                }\\n              }\\n            }\\n          }\\n        }\\n      }\\n    }\\n  \",\"operationName\":\"IntrospectionQuery\"}");
    }

    [Fact]
    public Task ParsingWithVariables()
    {
        return Parse("{\"query\":\"{\\n  noAttachment\\n  {\\n    argument\\n  }\\n}\",\"variables\":{\"key\":\"value\"},\"operationName\":\"theOperation\"}");
    }

    async Task Parse(string chars)
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chars)) {Position = 0};
        var (query, inputs, operation) = await RequestReader.ReadBody(stream, CancellationToken.None);
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