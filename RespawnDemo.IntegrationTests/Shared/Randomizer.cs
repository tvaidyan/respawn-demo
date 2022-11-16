namespace RespawnDemo.IntegrationTests.Shared;
public static class Randomizer
{
    public static string GetRandomString(int length = 10, string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        return new string(Enumerable.Repeat(allowedCharacters, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}
