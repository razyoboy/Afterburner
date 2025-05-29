using Afterburner.Services;
using Cocona;

namespace Afterburner.Commands;

public class FuelCommands
{
    [Command("unlimited-fuel", Aliases = ["uf"])]
    public async Task UnlimitedFuel(
        [Option("p", Description = "Path to the .miz file. If not provided, uses the default DCS Server path.")]
        string? path = null)
    {
        var finalPath = MizService.ResolveMizPath(path);
        await MizService.EnableUnlimitedFuel(finalPath);
        Console.WriteLine($"Patched: {finalPath}");
    }
}