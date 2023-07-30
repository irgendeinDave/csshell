namespace Autocompletion;

public class Autocomplete
{
    private int elementsToRequest;

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
        List<string> results = AllSuggestions(fullCommand);
        List<string> matchingResults = new();
        foreach (string result in results)
        {
            if (result.StartsWith(commandEnding))
                matchingResults.Add(result);
        }

        if (matchingResults.Count == 1)
            return results.ElementAt(0);
        else if (matchingResults.Count == 0)
            return "";
        else
        {
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
    }

    #region Methods to get List from different sources

    /// <summary>
    /// Finds all the appropriate suggestions depending on the given command
    /// does not check if the suggestions match the input
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
        return new();
    }

    // TODO: use relative paths for files and directories
    private static List<string> FilesInDirectory()
    {
        List<string> files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).ToList();
        for (int i = 0; i < files.Count; i++)
        {
            files[i] = RelativePath(files[i], Directory.GetCurrentDirectory());
        }

        return files;
    }

    private static List<string> Subdirectories()
    {
        List<string> dirs = Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).ToList();
        for (int i = 0; i < dirs.Count; i++)
        {
            dirs[i] = RelativePath(dirs[i], Directory.GetCurrentDirectory()) + "/";
        }

        return dirs;
    }

    #endregion

    private static string RelativePath(string fullPath, string pwd) => fullPath[(pwd.Length + 1)..];
}