using CommandExecution;

public static class Script
{
    private static Dictionary<string, List<string>> commandPackages = new();
    public static void runScript(string path, CommandRunner cr)
    {
        string[] lines = File.ReadAllLines(path);
        ParseFile(lines);
        
        //start to execute the main package
        //TODO: make sure that Main exists
        runCommandPackage("Main", cr);
        
        if (path != Settings.RcFilePath)
            Environment.Exit(0);
    }

    private static void ParseFile(string[] fileContent)
    {
        List<string> currentPackage = new();
        List<string> mainPackage = new(); // all commands that dont't belong in any package
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
                commandPackages.Add(packageName, currentPackage);
            }
            mainPackage.Add(fileContent[i]);
        }
        commandPackages.Add("Main", mainPackage);
    }

    private static string getFunctionName(string line)
    {
        return line.Substring(3);
    }

    private static void runCommandPackage(string name, CommandRunner cr)
    {
        foreach (var command in commandPackages["Main"])
        {
            if (command.StartsWith("run ") && commandPackages.ContainsKey(command.Substring(4)))
                runCommandPackage(command, cr);
                
            cr.runLine(command);
        }
    }
}

public record function(string Name, List<string> Package, List<string> Parameters);