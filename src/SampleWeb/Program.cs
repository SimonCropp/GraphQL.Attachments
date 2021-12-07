using Microsoft.AspNetCore;

public class Program
{
    public static Task Main()
    {
        var builder = WebHost.CreateDefaultBuilder();
        var currentDirectory = Directory.GetCurrentDirectory();
        builder.UseContentRoot(currentDirectory);
        builder.UseWebRoot(currentDirectory);
        var hostBuilder = builder.UseStartup<Startup>();
        return hostBuilder.Build().RunAsync();
    }
}