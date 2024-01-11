using CommandExecution;

public static class Script
{
    private static Dictionary<string, List<string>> commandPackages = new();
    public static void runScript(string path, CommandRunner cr)
    {
        string[] lines = File.ReadAllLines(path);
        ParseFile(lines);
        //TODO: Run commands

        if (path != Settings.RcFilePath)
            Environment.Exit(0);
    }

    private static void ParseFile(string[] fileContent)
    {
        List<string> currentPackage = new();
        
        for(int i = 0; i < fileContent.Length; ++i)
        {
            // functions
            if (fileContent[i].StartsWith("fn "))
            {
                //TODO: Test and maybe turn this into a method
                string packageName = getFunctionName(fileContent[i]);
                ++i;
                int count = 0;
                while (fileContent[i] != "end")
                {
                    currentPackage.Add(fileContent[i]);
                    ++i;
                }
                ++i;
                commandPackages.Add(getFunctionName(packageName), currentPackage);
            }
        }
    }

    private static string getFunctionName(string line)
    {
        return line.Substring(3);
    }
}