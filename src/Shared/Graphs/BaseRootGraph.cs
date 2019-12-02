using System.IO;
using GraphQL.Attachments;
using GraphQL.Types;

public abstract class BaseRootGraph : ObjectGraphType
{
    protected BaseRootGraph()
    {
        Name = GetType().Name;
        Field<ResultGraph>(
            "noAttachment",
            arguments: BuildArguments(),
            resolve: context => new Result
            {
                Argument = GetArgumentValue(context),
            });
        Field<ResultGraph>(
            "withAttachment",
            arguments: BuildArguments(),
            resolve: context =>
            {
                var incomingAttachments = context.IncomingAttachments();
                var outgoingAttachments = context.OutgoingAttachments();
                foreach (var incoming in incomingAttachments.Values)
                {
                    using var memoryStream = new MemoryStream();
                    incoming.CopyTo(memoryStream);
                    outgoingAttachments.AddBytes(incoming.Name, memoryStream.ToArray());
                }

                return new Result
                {
                    Argument = GetArgumentValue(context),
                };
            });
    }

    static string GetArgumentValue(ResolveFieldContext<object> context)
    {
        return context.GetArgument<string>("argument");
    }

    static QueryArguments BuildArguments()
    {
        return new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "argument"
            }
        );
    }
}