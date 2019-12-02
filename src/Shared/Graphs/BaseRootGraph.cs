using System.IO;
using GraphQL.Attachments;
using GraphQL.Types;

public abstract class BaseRootGraph :
    ObjectGraphType
{
    protected BaseRootGraph()
    {
        Name = GetType().Name;

        Field<ResultGraph>(
            "noAttachment",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "argument"
                }
            ),
            resolve: context => new Result
            {
                Argument = context.GetArgument<string>("argument"),
            });
        #region UsageInGraphs
        Field<ResultGraph>(
            "withAttachment",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "argument"
                }
            ),
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
                    Argument = context.GetArgument<string>("argument"),
                };
            });

        #endregion
    }
}