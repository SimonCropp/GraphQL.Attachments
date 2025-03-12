using Microsoft.Extensions.DependencyInjection;

public static class TestServices
{
    public static void AddGraphQlTestTypes(IServiceCollection services)
    {
        services.AddSingleton(new UploadGraphType());
        services.AddSingleton(new InputArgumentGraph());
        services.AddSingleton(new Query());
        services.AddSingleton(new Mutation());
        services.AddSingleton(new ResultGraph());
    }
}