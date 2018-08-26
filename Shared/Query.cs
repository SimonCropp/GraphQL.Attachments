﻿using System.Collections.Generic;
using System.Linq;
using GraphQL.Attachments;
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
        Field<ItemGraph>(
            "itemWithAttachment",
            resolve: context =>
            {
                var outgoingAttachments = context.OutgoingAttachments();
                outgoingAttachments.AddString("key", "foo");
                return items.First();
            });
    }
}