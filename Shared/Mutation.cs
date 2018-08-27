using System.Collections.Generic;
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
                var incomingAttachments = context.IncomingAttachments();
                foreach (var incoming in incomingAttachments.Values)
                {
                    using (var ms = new MemoryStream())
                    {
                        incoming.CopyTo(ms);
                        context.OutgoingAttachments().AddBytes(incoming.Name, ms.ToArray());
                    }
                }

                var item = context.GetArgument<Item>("item");
                items.Add(item);
                return new Result
                {
                    ItemCount = items.Count,
                    AttachmentCount = incomingAttachments.Count
                };
            });
    }
}