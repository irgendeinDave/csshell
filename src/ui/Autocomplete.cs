namespace Autocompletion;

public class Autocomplete
{
    private int elementsToRequest;
    private string commandEnding;

    public int ElementsToRequest
    {
        set { elementsToRequest = value; }
        get { return elementsToRequest; }
    }

    private List<string> files = new();

    public bool MoreThanOneElement()
    {
        files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).ToList();
        return files.Count > 1;
    }

    ///<summary>
    /// scan for available files in the working directory
    /// if there are multiple offer to print out all available ones and return an empty string
    /// else return the file name
    ///</summary>
    public string AutocompleteResult(string commandEnding, string fullCommand)
    {
        this.commandEnding = commandEnding;
        
        List<string> results = AllSuggestions(fullCommand);
        List<string> matchingResults = new();
        foreach (string result in results)
        {
            if (result.StartsWith(commandEnding))
                matchingResults.Add(result);
        }

        if (matchingResults.Count == 1)
            return matchingResults.ElementAt(0);
        if (matchingResults.Count == 0)
            return "";
       
        Console.WriteLine();
        if (matchingResults.Count > elementsToRequest)
        {
            Console.Write($"\n{matchingResults.Count} elements found. Show them all (y/n): ");
            ConsoleKeyInfo answer = Console.ReadKey();
            Console.WriteLine();
            if (answer.Key != ConsoleKey.Y)
            {
                Console.Write($"\n{Settings.Prompt()}" + fullCommand.ToString());
                return "";
            }
        }

        foreach (string element in matchingResults)
        {
            Console.Write($"{element}\t");
        }

        Console.Write($"\n{Settings.Prompt()}" + fullCommand.ToString());
        return "";
        
    }

    #region Methods to get List from different sources

    private readonly List<string> subdirectoriesCommands = new() { "ls", "cd", "find", "cp", "rmdir", "rm -r"};
    private readonly List<string> filesCommands = new() { "rm", "cat", "cp", "tail", "head" };
    
    /// <summary>
    /// Finds all the appropriate suggestions depending on the given command
    /// does not check if the suggestions match the input
    /// </summary>
    /// <param name="command">The full command</param>
    /// <returns> All suggestions for a specific command name</returns>
    private List<string> AllSuggestions(string command)
    {
        List<string> results = new();
        string commandName = command.Split(" ", 2).First();
        if (subdirectoriesCommands.Contains(commandName))
            results.AddRange(Subdirectories());
        if (filesCommands.Contains(commandName))
        {
            results.AddRange(FilesInDirectory());
            results.AddRange(Subdirectories()); //include also files in subdirectories -> add subdirectories for commands that need files
        }
        return results;
    }

    private  List<string> GitCommands()
    {
        return new();
    }

    private string relativeSearchPath()
    {
        int lastPathSeparatorPosition = commandEnding.LastIndexOf('/');
        if (lastPathSeparatorPosition == -1)
            return "";
        return commandEnding.Substring(0, lastPathSeparatorPosition);
    }
    
    private List<string> FilesInDirectory()
    {
        List<string> files = Directory.EnumerateFiles($"{Directory.GetCurrentDirectory()}/{relativeSearchPath()}").ToList();
        for (int i = 0; i < files.Count; i++)
        {
            files[i] = Util.RelativePath(files[i], Directory.GetCurrentDirectory());
        }

        return files;
    }

    private List<string> Subdirectories()
    {
        List<string> dirs = Directory.EnumerateDirectories($"{Directory.GetCurrentDirectory()}/{relativeSearchPath()}").ToList();
        for (int i = 0; i < dirs.Count; i++)
        {
            dirs[i] = Util.RelativePath(dirs[i], Directory.GetCurrentDirectory()) + "/";
        }

        return dirs;
    }
    // TODO: add git commands

    #endregion
}