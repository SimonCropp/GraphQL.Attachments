using GraphQL;
using GraphQL.Types;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        TestServices.AddGraphQlTestTypes(services);

        services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        services.AddSingleton<ISchema, Schema>();
        services.AddSingleton<GraphQlMiddleware>();
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseWhen(
            context =>
            {
                var path = context.Request.Path;
                return path.StartsWithSegments("/graphql", out var remaining) &&
                       string.IsNullOrEmpty(remaining);
            },
            b => b.UseMiddleware<GraphQlMiddleware>());
        builder.UseStaticFiles();
    }
}