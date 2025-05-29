using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
namespace Afterburner.Services;

public static class MizService
{
    /// <summary>
    /// Enables unlimited fuel by setting ["fuel"] = true in the misson.lua inside the .miz archive.
    /// This version reads and writes entirely in-memory (no temp folder), then writes to a specified output path.
    /// Requires ICSharpCode.SharpZipLib (for HostSystem preservation).
    /// </summary>
    /// <param name="inputMizPath">Path to the source .miz file.</param>
    /// <param name="outputMizPath">Path to write the modified .miz file.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task EnableUnlimitedFuel(string inputMizPath, string outputMizPath, CancellationToken cancellationToken = default)
    {
        await using var fsIn = File.OpenRead(inputMizPath);
        await using var fsOut = File.Create(outputMizPath);

        using var zipIn = new ZipFile(fsIn);
        await using var zipOut = new ZipOutputStream(fsOut)
        {
            IsStreamOwner = true,
            UseZip64 = UseZip64.Off
        };
        zipOut.SetLevel(Deflater.BEST_SPEED);

        foreach (ZipEntry entry in zipIn)
        {
            if (!entry.IsFile)
            {
                continue;
            }

            await using var entryStream = zipIn.GetInputStream(entry);
            if (entry.Name.EndsWith("mission", StringComparison.OrdinalIgnoreCase))
            {
                var text = await new StreamReader(entryStream, Encoding.UTF8).ReadToEndAsync(cancellationToken);

                // voo-doo magic
                var patched = Regex.Replace(text,
                    @"(\[\s*""forcedOptions""\s*\]\s*=\s*\{\s*)([^}]*)",
                    match =>
                    {
                        var beforeContent = match.Groups[1].Value;
                        var innerContent = match.Groups[2].Value;

                        if (Regex.IsMatch(innerContent, @"\[\s*""fuel""\s*\]"))
                        {
                            // Fuel option exists — replace its value
                            innerContent = Regex.Replace(innerContent,
                                @"\[\s*""fuel""\s*\]\s*=\s*\w+",
                                "[\"fuel\"]=true");
                        }
                        else
                        {
                            // Inject new fuel option
                            if (!string.IsNullOrWhiteSpace(innerContent.Trim()))
                            {
                                innerContent = $"[\"fuel\"]=true,\n\t\t" + innerContent;
                            }
                            else
                            {
                                innerContent = "[\"fuel\"]=true,\n\t\t";
                            }
                        }

                        return beforeContent + innerContent;
                    },
                    RegexOptions.Singleline);


                var newEntry = new ZipEntry(entry.Name)
                {
                    DateTime = entry.DateTime,
                    CompressionMethod = CompressionMethod.Deflated,
                    HostSystem = 0,

                };

                await zipOut.PutNextEntryAsync(newEntry, cancellationToken);

                var data = Encoding.UTF8.GetBytes(patched);
                await zipOut.WriteAsync(data, cancellationToken);
            }
            else
            {
                var newEntry = new ZipEntry(entry.Name)
                {
                    DateTime = entry.DateTime,
                    CompressionMethod = CompressionMethod.Deflated,
                    HostSystem = 0
                };
                await zipOut.PutNextEntryAsync(newEntry, cancellationToken);
                StreamUtils.Copy(entryStream, zipOut, new byte[4096]);
            }

            await zipOut.CloseEntryAsync(cancellationToken);
        }
        await zipOut.FinishAsync(cancellationToken);
    }

    public static async Task EnableUnlimitedFuel(string inputMizPath, CancellationToken cancellationToken = default)
    {
        var tempFileName = Path.GetRandomFileName();
        var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
        await EnableUnlimitedFuel(inputMizPath, tempFilePath, cancellationToken);

        // replace the original file with the modified one
        if (File.Exists(inputMizPath))
        {
            File.Delete(inputMizPath);
            File.Move(tempFilePath, inputMizPath);
        }
    }

    public static string ResolveMizPath(string? suppliedPath = null)
    {
        if (!string.IsNullOrWhiteSpace(suppliedPath))
        {
            return ExpandHome(suppliedPath);
        }

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(userProfilePath, "Saved Games", "DCS.dcs_serverrelease", "Missions", "liberation_nextturn.miz");
    }

    private static string ExpandHome(string path)
    {
        if (path.StartsWith('~'))
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var rest = path.TrimStart('~', '/').TrimStart('\\');
            return Path.Combine(home, rest);
        }
        return path;
    }
}
