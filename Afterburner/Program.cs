using Afterburner.Commands;
using Cocona;

namespace Afterburner;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder();
        var app = builder.Build();
        app.AddCommands<FuelCommands>();

        await app.RunAsync();
    }
}
