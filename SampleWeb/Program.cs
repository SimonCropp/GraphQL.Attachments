using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

public class Program
{
    public static void Main()
    {
        var builder = WebHost.CreateDefaultBuilder();
        var currentDirectory = Directory.GetCurrentDirectory();
        builder.UseContentRoot(currentDirectory);
        builder.UseWebRoot(currentDirectory);
        var hostBuilder = builder.UseStartup<Startup>();
        hostBuilder.Build().Run();
    }
}