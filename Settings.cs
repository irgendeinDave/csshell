using cmd;

public static class Settings
{
    private static readonly string UserHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static readonly string HistoryFilePath = $"{UserHome}/.csshellhist";
    public static readonly string RcFilePath = $"{UserHome}/.csshellrc";

    public static  string Prompt() => (Environment.GetEnvironmentVariable("PROMPT") == null
        ? "$ "
        : Environment.GetEnvironmentVariable("PROMPT"))!;
}