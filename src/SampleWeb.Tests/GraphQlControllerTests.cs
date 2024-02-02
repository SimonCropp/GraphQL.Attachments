public class GraphQlControllerTests
{
    static QueryExecutor executor;
    static HttpClient client;

    static GraphQlControllerTests()
    {
        var server = GetTestServer();
        client = server.CreateClient();
        executor = new(client);
    }

    [Fact]
    public Task GetIntrospection() =>
        Verify(executor.ExecuteGet(IntrospectionQuery));

    [Fact]
    public Task PostIntrospection() =>
        Verify(executor.ExecutePost(IntrospectionQuery));

    [Fact]
    public Task Get()
    {
        var query = """
                    {
                      noAttachment (argument: "argumentValue")
                      {
                        argument
                      }
                    }
                    """;
        return Verify(executor.ExecuteGet(query));
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
    public async Task Post()
    {
        var mutation = """
                       mutation
                       {
                         withAttachment (argument: "argumentValue")
                         {
                           argument
                         }
                       }
                       """;
        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    [Fact]
    public async Task Post_with_attachment()
    {
        var mutation = """
                       mutation
                       {
                         withAttachment (argument: "argumentValue")
                         {
                           argument
                         }
                       }
                       """;

        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        content.AddContent("key", "foo");
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    [Fact]
    public async Task Post_with_multiple_attachment()
    {
        var mutation = """
                       mutation
                       {
                         withAttachment (argument: "argumentValue")
                         {
                           argument
                         }
                       }
                       """;

        using var content = new MultipartFormDataContent();
        content.AddQueryAndVariables(mutation);
        content.AddContent("key1", "foo1");
        content.AddContent("key2", "foo2");
        using var response = await client.PostAsync("graphql", content);
        await Verify(response.ProcessResponse());
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new(hostBuilder);
    }

    public static readonly string IntrospectionQuery =
        """
          query IntrospectionQuery {
            __schema {
              description
              queryType { name }
              mutationType { name }
              subscriptionType { name }
              types {
                ...FullType
              }
              directives {
                name
                description
                locations
                args {
                  ...InputValue
                }
              }
            }
          }

          fragment FullType on __Type {
            kind
            name
            description
            fields(includeDeprecated: true) {
              name
              description
              args {
                ...InputValue
              }
              type {
                ...TypeRef
              }
              isDeprecated
              deprecationReason
            }
            inputFields {
              ...InputValue
            }
            interfaces {
              ...TypeRef
            }
            enumValues(includeDeprecated: true) {
              name
              description
              isDeprecated
              deprecationReason
            }
            possibleTypes {
              ...TypeRef
            }
          }

          fragment InputValue on __InputValue {
            name
            description
            type { ...TypeRef }
            defaultValue
          }

          fragment TypeRef on __Type {
            kind
            name
            ofType {
              kind
              name
              ofType {
                kind
                name
                ofType {
                  kind
                  name
                  ofType {
                    kind
                    name
                    ofType {
                      kind
                      name
                      ofType {
                        kind
                        name
                        ofType {
                          kind
                          name
                        }
                      }
                    }
                  }
                }
              }
            }
          }

        """;
}