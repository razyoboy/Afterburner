﻿using System.Text;
using Afterburner.Services;
using ICSharpCode.SharpZipLib.Zip;
using Xunit.Abstractions;
namespace Afterburner.Tests.Services;

public class MizServiceTests(ITestOutputHelper output)
{
    [Fact]
    public async Task EnableUnlimitedFuel_ShouldModifyMissionLua()
    {
        // Arrange
        const string inputMizPath = "Data/liberation_nextturn.miz";
        const string outputMizPath = "Data/modified.miz";

        try
        {
            // Act
            await MizService.EnableUnlimitedFuel(inputMizPath, outputMizPath);

            // Assert
            string content;
            using var zip = new ZipFile(outputMizPath);
            {
                var optionsEntry = zip.GetEntry("mission");
                Assert.NotNull(optionsEntry);

                await using var stream = zip.GetInputStream(optionsEntry);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                content = await reader.ReadToEndAsync();
                output.WriteLine(content);
            }

            Assert.Contains("[\"fuel\"]=true", content);
        }
        finally
        {
            // Clean up
            if (File.Exists(outputMizPath))
            {
                File.Delete(outputMizPath);
            }
        }
    }

    [Fact]
    public async Task EnableUnlimitedFuel_ShouldFail_FileNotFound()
    {
        // Arrange
        const string inputMizPath = "bruh.hello.miz";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await MizService.EnableUnlimitedFuel(inputMizPath, Path.GetRandomFileName());
        });
    }
}
