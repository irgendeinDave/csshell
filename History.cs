namespace CommandHistory;

public static class History
{
    private static readonly string historyFilePath = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.csshellhist");

    public static void Append(string command)
    {
        if (string.IsNullOrEmpty(command))
            return;

        if (LastStoredCommand() == command)
            return;

       File.AppendAllText(historyFilePath, $"{command}\n");
    }

    public static string LastStoredCommand()
    {
        if (new FileInfo(historyFilePath).Length == 0)
            return string.Empty;
        return File.ReadLines(historyFilePath).Last();
    }
}