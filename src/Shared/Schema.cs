using System;

public class Schema :
    GraphQL.Types.Schema
{
    public Schema(IServiceProvider provider) :
        base(provider)
    {
        Query = (Query) provider.GetService(typeof(Query));
        Mutation = (Mutation) provider.GetService(typeof(Mutation));
    }
}