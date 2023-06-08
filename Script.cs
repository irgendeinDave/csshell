using System.IO;
using cmd;


public static class Script
{
    public static void runScript(string path, CommandRunner cr)
    {
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            if (!line.StartsWith("#!"))
                cr.run(line);
        }
        Environment.Exit(0);
    }
}