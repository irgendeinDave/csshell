namespace CommandHistory;

public static class History
{
    private static readonly string historyFilePath =
        new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.csshellhist");

    public static void Append(string command)
    {
        if (!File.Exists(historyFilePath))
            using (File.Create(historyFilePath))
            {
            }

        if (string.IsNullOrEmpty(command) || command.IndexOf("!!") > -1)
            return;

        if (StoredCommand(0) == command)
            return;

        File.AppendAllText(historyFilePath, $"{command}\n");
    }

    public static string StoredCommand(int index)
    {
        if (new FileInfo(historyFilePath).Length == 0)
            return string.Empty;

        var commands = File.ReadLines(historyFilePath).ToList();
        if (index >= commands.Count)
            return commands.First().Trim();
        return commands.ElementAt(commands.Count - (index + 1)).Trim();
    }
}