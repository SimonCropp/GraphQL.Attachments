using System.Collections.Generic;
using GraphQL.Attachments;
using Microsoft.Extensions.DependencyInjection;

public static class TestServices
{
    static TestServices()
    {
        ContextualAttachments.SetContextFunc(o => (AttachmentContext)o);
    }
    public static void AddGraphQlTestTypes(List<Item> items, IServiceCollection services)
    {
        services.AddSingleton(new ItemGraph());
        services.AddSingleton(new ItemInput());
        services.AddSingleton(new Query(items));
        services.AddSingleton(new Mutation(items));
        services.AddSingleton(new ResultGraph());
    }
}