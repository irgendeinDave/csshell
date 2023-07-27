namespace Autocompletion;

public class Autocomplete
{
    private List<string> files = new();

    public int ElementsToRequest { set; get; }

    public bool MoreThanOneElement()
    {
        files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).ToList();
        return files.Count > 1;
    }

    /// <summary>
    ///     scan for available files in the working directory
    ///     if there are multiple offer to print out all available ones and return an empty string
    ///     else return the file name
    /// </summary>
    public string AutocompleteResult(string commandEnding, string fullCommand)
    {
        var results = AllSuggestions(fullCommand);
        List<string> matchingResults = new();
        foreach (var result in results)
            if (result.StartsWith(commandEnding))
                matchingResults.Add(result);
        if (matchingResults.Count == 1) return results.ElementAt(0);

        if (matchingResults.Count == 0)
        {
            return "";
        }

        Console.WriteLine();
        if (matchingResults.Count > ElementsToRequest)
        {
            // TODO: print actual prompt instaead of "$ "
            Console.Write($"\n{matchingResults.Count} elements found. Show them all (y/n): ");
            var answer = Console.ReadKey();
            Console.WriteLine();
            if (answer.Key != ConsoleKey.Y)
            {
                Console.Write("\n$ " + fullCommand);
                return "";
            }
        }

        foreach (var element in matchingResults) Console.Write($"{element}\t");
        Console.Write("\n$ " + fullCommand);
        return "";
    }

    private static string RelativePath(string fullPath, string pwd)
    {
        return fullPath[(pwd.Length + 1)..];
    }

    #region Methods to get List from different sources

    /// <summary>
    ///     Finds all the appropriate suggestions depending on the given command
    ///     does not check if the suggestions match the input
    /// </summary>
    /// <param name="command">The full command</param>
    /// <returns> All suggestions for a specific command name</returns>
    private List<string> AllSuggestions(string command)
    {
        List<string> results = new();
        if (command.StartsWith("ls"))
            results.AddRange(Subdirectories());
        else if (command.StartsWith("cd"))
            results.AddRange(Subdirectories());
        return results;
    }

    private static List<string> GitCommands()
    {
        return new List<string>();
    }

    // TODO: use relative paths for files and directories
    private static List<string> FilesInDirectory()
    {
        var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).ToList();
        for (var i = 0; i < files.Count; i++) files[i] = RelativePath(files[i], Directory.GetCurrentDirectory());
        return files;
    }

    private static List<string> Subdirectories()
    {
        var dirs = Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).ToList();
        for (var i = 0; i < dirs.Count; i++) dirs[i] = RelativePath(dirs[i], Directory.GetCurrentDirectory()) + "/";
        return dirs;
    }

    #endregion
}