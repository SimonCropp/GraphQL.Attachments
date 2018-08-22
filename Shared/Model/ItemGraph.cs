using GraphQL.Types;

public class ItemGraph : ObjectGraphType<Item>
{
    public ItemGraph()
    {
        Field(d => d.Name);
    }
}