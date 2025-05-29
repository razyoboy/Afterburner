using Afterburner.Services;
using Cocona;

namespace Afterburner;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CoconaApp.RunAsync<Program>(args);
    }

    [Command("unlimited-fuel")]
    public async Task UnlimitedFuel([Option("path")] string? path = null)
    {
        var finalPath = MizService.ResolveMizPath(path);
        await MizService.EnableUnlimitedFuel(finalPath);
        Console.WriteLine($"Patched {finalPath}");
    }
}