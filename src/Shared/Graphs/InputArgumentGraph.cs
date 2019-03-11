using GraphQL.Types;

public class InputArgumentGraph : InputObjectGraphType<InputArgument>
{
    public InputArgumentGraph()
    {
        Field(x => x.Value);
    }
}