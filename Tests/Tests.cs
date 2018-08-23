using System.Collections.Generic;
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
        var mutation = @"mutation ($item:ItemInput!){ addItem(item: $item) { itemCount byteCount } }";
        var jObject = JObject.Parse(@"
{
  ""item"":
  {
    ""name"": ""TheName"",
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
            new Item {Name = "theName"}
        };
        var result = await RunQuery(queryString, items);
        ObjectApprover.VerifyWithJson(result);
    }

    static async Task<object> RunQuery(string queryString, List<Item> items, Inputs inputs = null)
    {
        var services = new ServiceCollection();

        TestServices.AddGraphQlTestTypes(items, services);

        return await QueryExecutor.ExecuteQuery(queryString, services, inputs);
    }
}