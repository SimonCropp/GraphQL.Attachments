using System.Collections.Generic;
using GraphQL;
using GraphQL.Attachments;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        GraphQLAttachmentConventions.RegisterInContainer((type, instance) => { services.AddSingleton(type, instance); });

        TestServices.AddGraphQlTestTypes(
            new List<Item>
            {
                new Item
                {
                    Name = "TheName"
                }
            },
            services);

        services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        services.AddSingleton<IDependencyResolver>(
            provider => new FuncDependencyResolver(provider.GetRequiredService));
        services.AddSingleton<ISchema, Schema>();
        var mvc = services.AddMvc();
        mvc.SetCompatibilityVersion(CompatibilityVersion.Latest);
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseStaticFiles();
        builder.UseMvc();
    }
}