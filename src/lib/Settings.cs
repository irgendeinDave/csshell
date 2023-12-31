using CommandProcessing;

public static class Settings
{
    public static readonly string HistoryFilePath = $"{Util.UserHome}/.csshellhist";
    public static readonly string RcFilePath = $"{Util.UserHome}/.csshellrc";

    public static  string Prompt() => CommandProcessor.processArguments(Environment.GetEnvironmentVariable("PROMPT") == null
        ? "$ "
        : Environment.GetEnvironmentVariable("PROMPT")!);
}