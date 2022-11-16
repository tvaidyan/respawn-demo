namespace RespawnDemo.IntegrationTests.Shared;
public static class FileSystemUtilities
{
    public static string ConvertToPosix(this string filePath)
    {
        if (!OperatingSystem.IsWindows())
            return filePath;

        // C:\one\two\three => /c/one/two/three
        return "/" + filePath[..1].ToLower() + filePath[2..].Replace(@"\", "/")
            .Replace(@":", "");
    }
}