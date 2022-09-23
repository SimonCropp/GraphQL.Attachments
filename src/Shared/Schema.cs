using Microsoft.Extensions.DependencyInjection;

public class Schema :
    GraphQL.Types.Schema
{
    public Schema(IServiceProvider provider) :
        base(provider)
    {
        Query = (Query) provider.GetRequiredService(typeof(Query));
        Mutation = (Mutation) provider.GetRequiredService(typeof(Mutation));
    }
}