using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

public class Query : ObjectGraphType<object>
{
    public Query(List<Item> items)
    {
        Name = "Query";

        Field<ItemGraph>(
            "item",
            resolve: context => items.First()
        );
    }
}