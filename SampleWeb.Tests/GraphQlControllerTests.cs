﻿using System.Text;
using System.Threading.Tasks;
using GraphQL.Attachments;
using GraphQL.Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

public class GraphQlControllerTests
{
    ClientQueryExecutor queryExecutor;

    public GraphQlControllerTests()
    {
        var server = GetTestServer();
        var client = server.CreateClient();
        queryExecutor = new ClientQueryExecutor(client);
    }

    [Fact]
    public async Task GetIntrospection()
    {
        var response = await queryExecutor.ExecuteGet(SchemaIntrospection.IntrospectionQuery);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Contains("addItem", result);
        Assert.Contains("item", result);
    }

    [Fact]
    public async Task Get()
    {
        var query = @"
{
  item
  {
    name
  }
}";
        var response = await queryExecutor.ExecuteGet(query);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"data\":{\"item\":{\"name\":\"TheName\"}}}", result);
    }

    [Fact]
    public async Task Post()
    {
        var mutation = "mutation ($item:ItemInput!){ addItem(item: $item) { itemCount byteCount } }";
        var variables = new
        {
            item = new
            {
                name = "TheName"
            }
        };
        var response = await queryExecutor.ExecutePost(
            mutation,
            variables,
            action: context => { context.AddAttachment("key", Encoding.UTF8.GetBytes("foo")); });

        Assert.Equal("{\"data\":{\"addItem\":{\"itemCount\":2,\"byteCount\":3}}}", response.ResultStream.ConvertToString());
    }

    static TestServer GetTestServer()
    {
        var hostBuilder = new WebHostBuilder();
        hostBuilder.UseStartup<Startup>();
        return new TestServer(hostBuilder);
    }
}