using cmd;

public static class Settings
{
    private static string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static readonly string historyFilePath = $"{userHome}/.csshellhist";
    public static readonly string rcFilePath = $"{userHome}/.csshellrc";
}