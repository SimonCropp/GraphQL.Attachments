using GraphQL;
using GraphQL.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((_, services) =>
{
    TestServices.AddGraphQlTestTypes(services);
    services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
    services.AddSingleton<ISchema, Schema>();
    services.AddSingleton<GraphQlMiddleware>();
    services.AddCors();
});

await using var app = builder.Build();
#if DEBUG
app.UseCors(cors =>
{
    cors.AllowAnyHeader();
    cors.AllowAnyMethod();
    cors.SetIsOriginAllowed(_ => true);
});
#endif
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseWhen(context =>
    {
        var path = context.Request.Path;
        return path.StartsWithSegments("/graphql", out var remaining) &&
               string.IsNullOrEmpty(remaining);
    },
    b => b.UseMiddleware<GraphQlMiddleware>());


await app.RunAsync();

// Add this to allow WebApplicationFactory to reference Program
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once RedundantTypeDeclarationBody
public partial class Program {}