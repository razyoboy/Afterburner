using Afterburner.Utils;
namespace Afterburner.Options;

public class ApplicationOptions
{
    public string MizPath { get; set; } = FileUtils.GetDefaultMizPath();
}
