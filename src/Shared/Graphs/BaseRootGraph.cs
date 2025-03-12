using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;

public abstract class BaseRootGraph :
    ObjectGraphType
{
    protected BaseRootGraph()
    {
        Name = GetType().Name;

        Field<ResultGraph>("noAttachment")
            .Argument<NonNullGraphType<StringGraphType>>("argument")
            .Resolve(context => new Result
            {
                Argument = context.GetArgument<string>("argument"),
            });

        #region UsageInGraphs

        Field<ResultGraph>("withAttachment")
            .Argument<NonNullGraphType<StringGraphType>>("argument")
            .Resolve(context =>
            {
                var incomingAttachments = context.IncomingAttachments();
                var outgoingAttachments = context.OutgoingAttachments();

                foreach (var incoming in incomingAttachments.Values)
                {
                    // For sample purpose echo the incoming request
                    // stream to the outgoing response stream
                    var memoryStream = new MemoryStream();
                    incoming.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    outgoingAttachments.AddStream(incoming.Name, memoryStream);
                }

                return new Result
                {
                    Argument = context.GetArgument<string>("argument"),
                };
            });
        Field<ResultGraph>("withAttachmentAsInput")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "argument"
                },
                new QueryArgument<UploadGraphType>
                {
                    Name = "file"
                }))
            .Resolve(context =>
            {
                var incomingAttachments = context.IncomingAttachments();
                var fileName = "";

                foreach (var incoming in incomingAttachments.Values)
                {
                    var memoryStream = new MemoryStream();
                    incoming.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    fileName = incoming.Name;
                }

                // This mutation is built for apollo client hence not returning file back
                // to client
                return new Result
                {
                    Argument = fileName
                };
            });

        #endregion
    }
}