using System.Collections.Generic;
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
                },
                new QueryArgument<NonNullGraphType<AttachmentInput>>
                {
                    Name = "attachment",
                }
            ),
            resolve: context =>
            {
                var attachments = context.GetArgument<Attachment>("attachment");
                var item = context.GetArgument<Item>("item");
                items.Add(item);
                return new Result {Count = items.Count};
            });
    }
}