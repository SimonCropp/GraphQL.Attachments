using GraphQL.Types;

public class InputArgumentGraph :
    InputObjectGraphType<InputArgument>
{
    public InputArgumentGraph() =>
        Field(_ => _.Value);
}