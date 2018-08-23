using GraphQL.Types;

public class ResultGraph : ObjectGraphType<Result>
{
    public ResultGraph()
    {
        Field<IntGraphType>("itemCount");
        Field<IntGraphType>("byteCount");
    }
}