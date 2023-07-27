using System.IO;
using cmd;


public static class Script
{
    public static void runScript(string path, CommandRunner cr)
    {
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            cr.runLine(line);
        }
        if (path != Settings.rcFilePath)
            Environment.Exit(0);
    }
}