using GraphQL.Types;

public class ItemInput : InputObjectGraphType<Item>
{
    public ItemInput()
    {
        Field<NonNullGraphType<StringGraphType>>("name");
    }
}