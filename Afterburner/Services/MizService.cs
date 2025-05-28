using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
namespace Afterburner.Services;

public class MizService
{
    /// <summary>
    /// Enables unlimited fuel by setting ["fuel"] = true in the options.lua inside the .miz archive.
    /// This version reads and writes entirely in-memory (no temp folder), then writes to a specified output path.
    /// Requires ICSharpCode.SharpZipLib (for HostSystem preservation).
    /// </summary>
    /// <param name="inputMizPath">Path to the source .miz file.</param>
    /// <param name="outputMizPath">Path to write the modified .miz file.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task EnableUnlimitedFuel(string inputMizPath, string outputMizPath, CancellationToken cancellationToken = default)
    {
        await using var fsIn = File.OpenRead(inputMizPath);
        await using var fsOut = File.Create(outputMizPath);

        using var zipIn = new ZipFile(fsIn);
        await using var zipOut = new ZipOutputStream(fsOut)
        {
            IsStreamOwner = true
        };
        zipOut.SetLevel(Deflater.BEST_SPEED);

        foreach (ZipEntry entry in zipIn)
        {
            if (!entry.IsFile)
            {
                continue;
            }

            await using var entryStream = zipIn.GetInputStream(entry);
            if (entry.Name.EndsWith("options", StringComparison.OrdinalIgnoreCase))
            {
                var text = await new StreamReader(entryStream, Encoding.UTF8).ReadToEndAsync(cancellationToken);

                // voo-doo magic
                var patched = Regex.Replace(text,
                    @"\[""fuel""\]\s*=\s*\w+",
                    "[\"fuel\"] = true");

                var newEntry = new ZipEntry(entry.Name)
                {
                    DateTime = entry.DateTime,
                    CompressionMethod = CompressionMethod.Deflated,
                    HostSystem = entry.HostSystem
                };

                await zipOut.PutNextEntryAsync(newEntry, cancellationToken);

                var data = Encoding.UTF8.GetBytes(patched);
                await zipOut.WriteAsync(data, cancellationToken);
            }
            else
            {
                var newEntry = new ZipEntry(entry)
                {
                    CompressionMethod = CompressionMethod.Deflated,
                    HostSystem = entry.HostSystem
                };
                await zipOut.PutNextEntryAsync(newEntry, cancellationToken);
                StreamUtils.Copy(entryStream, zipOut, new byte[4096]);
            }

            await zipOut.CloseEntryAsync(cancellationToken);
            await zipOut.FinishAsync(cancellationToken);
        }
    }
}
