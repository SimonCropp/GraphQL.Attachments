using GraphQL.Types;

public class ResultGraph : ObjectGraphType<Result>
{
    public ResultGraph()
    {
        Field(x => x.ItemCount);
        Field(x => x.AttachmentCount);
    }
}