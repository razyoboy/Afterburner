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
        var options = SettingsService.Load();
        await MizService.EnableUnlimitedFuel(options.MizPath);
        Console.WriteLine($"Patched: {options.MizPath}");
    }
}