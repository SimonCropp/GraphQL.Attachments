﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using ObjectApproval;
using Xunit;

public class Tests
{
    [Fact]
    public async Task Mutation()
    {
        var mutation = @"
mutation ($item:ItemInput!)
{
  addItem(item: $item)
  {
    itemCount attachmentCount
  }
}";
        var jObject = JObject.Parse(@"
{
  'item':
  {
    'name': 'TheName',
  }
}");

        var items = new List<Item>();
        await RunQuery(mutation, items, jObject.ToInputs());
        ObjectApprover.VerifyWithJson(items);
    }

    [Fact]
    public async Task Query()
    {
        var queryString = @"
{
  item
  {
    name
  }
}";
        var items = new List<Item>
        {
            new Item
            {
                Name = "theName"
            }
        };
        var result = await RunQuery(queryString, items, null);
        ObjectApprover.VerifyWithJson(result);
    }

    static Task<AttachmentExecutionResult> RunQuery(string queryString, List<Item> items, Inputs inputs)
    {
        var services = new ServiceCollection();

        TestServices.AddGraphQlTestTypes(items, services);

        return QueryExecutor.ExecuteQuery(queryString, services, inputs);
    }
}