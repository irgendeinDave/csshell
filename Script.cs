using cmd;

public static class Script
{
    public static void runScript(string path, CommandRunner cr)
    {
        var lines = File.ReadAllLines(path);
        foreach (var line in lines) cr.runLine(line);
        if (path != Settings.rcFilePath)
            Environment.Exit(0);
    }
}