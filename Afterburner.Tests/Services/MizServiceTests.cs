using System.Text;
using Afterburner.Services;
using ICSharpCode.SharpZipLib.Zip;
using Xunit.Abstractions;
namespace Afterburner.Tests.Services;

public class MizServiceTests(ITestOutputHelper output)
{
    private readonly MizService _mizService = new MizService();

    [Fact]
    public async Task EnableUnlimitedFuel_ShouldModifyOptionsLua()
    {
        // Arrange
        const string inputMizPath = "Data/liberation_nextturn.miz";
        const string outputMizPath = "Data/modified.miz";

        try
        {
            // Act
            await _mizService.EnableUnlimitedFuel(inputMizPath, outputMizPath);

            // Assert
            string content;
            using var zip = new ZipFile(outputMizPath);
            {
                var optionsEntry = zip.GetEntry("options");
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
}
