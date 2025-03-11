using GraphQL;
using GraphQL.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((_, services) =>
{
    TestServices.AddGraphQlTestTypes(services);
    services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
    services.AddSingleton<ISchema, Schema>();
    services.AddSingleton<GraphQlMiddleware>();
});

await using var app = builder.Build();
app.UseWhen(context =>
    {
        var path = context.Request.Path;
        return path.StartsWithSegments("/graphql", out var remaining) &&
               string.IsNullOrEmpty(remaining);
    },
    b => b.UseMiddleware<GraphQlMiddleware>());
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
await app.RunAsync();