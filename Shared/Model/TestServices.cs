using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public static class TestServices
{
    public static void AddGraphQlTestTypes(List<Item> items, IServiceCollection services)
    {
        services.AddSingleton(new ItemGraph());
        services.AddSingleton(new ItemInput());
        services.AddSingleton(new Query(items));
        services.AddSingleton(new Mutation(items));
        services.AddSingleton(new ResultGraph());
        services.AddSingleton(new AttachmentInput());
        services.AddSingleton(new StreamGraphType());
    }
}