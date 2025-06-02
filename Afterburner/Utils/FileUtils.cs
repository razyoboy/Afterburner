namespace Afterburner.Utils;

public static class FileUtils
{
    public static void FileExistsOrThrow(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");
        }
    }

    public static string GetDefaultMizPath()
    {
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(userProfilePath, "Saved Games", "DCS.dcs_serverrelease", "Missions", "liberation_nextturn.miz");
    }
}
