using GraphQL;
using GraphQL.Types;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        TestServices.AddGraphQlTestTypes(services);

        services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        services.AddSingleton<ISchema, Schema>();
        var mvc = services.AddMvc(option => option.EnableEndpointRouting = false);
        mvc.AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseStaticFiles();
        builder.UseMvc();
    }
}