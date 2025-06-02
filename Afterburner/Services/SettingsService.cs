using System.Text.Json;
using Afterburner.Options;
using Afterburner.Utils;
namespace Afterburner.Services;

public static class SettingsService
{
    private const string SettingsFileName = "afterburner.settings.json";
    private static readonly string SettingsFilePath = Path.Combine(
        AppContext.BaseDirectory,
        SettingsFileName
    );

    public static ApplicationOptions Load()
    {
        if (!File.Exists(SettingsFilePath))
        {
            // create and save then return default options
            var defaultOptions = new ApplicationOptions();
            defaultOptions.Save();

            return defaultOptions;
        }

        var jsonString = File.ReadAllText(SettingsFilePath);
        var options = JsonSerializer.Deserialize<ApplicationOptions>(jsonString, JsonOptions.Default());
        if (options is null)
        {
            throw new InvalidOperationException("Failed to deserialize application options.");
        }

        return options;
    }

    public static void Save(this ApplicationOptions options)
    {
        var jsonString = JsonSerializer.Serialize(options, JsonOptions.Default());
        File.WriteAllText(SettingsFilePath, jsonString);
    }
}
