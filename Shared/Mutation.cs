﻿using System.Collections.Generic;
using System.IO;
using GraphQL.Attachments;
using GraphQL.Types;

public class Mutation : ObjectGraphType
{
    public Mutation(List<Item> items)
    {
        Name = "Mutation";
        Field<ResultGraph>(
            "addItem",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<ItemInput>>
                {
                    Name = "item"
                }
            ),
            resolve: context =>
            {
                long length = 0;
                var incomingAttachments = context.Attachments();
                if (incomingAttachments.TryRead(out var func))
                {
                    using (var attachment = func())
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        length = ms.Length;
                    }
                }

                var item = context.GetArgument<Item>("item");
                items.Add(item);
                return new Result
                {
                    ItemCount = items.Count,
                    ByteCount = length
                };
            });
    }
}