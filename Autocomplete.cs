namespace Autocompletion;

public class Autocomplete
{
    private int elementsToRequest;
    public int ElementsToRequest
    {
        set { elementsToRequest = value; }
        get { return elementsToRequest; }
    }

    private List<string> files = new ();

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
    public string AutocompleteResult(string commandStart, string currentCommand)
    {
        string pwd = Directory.GetCurrentDirectory();
        files = Directory.EnumerateFiles(pwd).ToList();
        List<string> directories = Directory.EnumerateDirectories(pwd).ToList();
        List<string> results = files;
        results.AddRange(directories);
        List<string> matchingResults = new ();
        foreach (string result in results)
        {
            if (RelativePath(result, pwd).StartsWith(commandStart))
                matchingResults.Add(result);
        }
        if (matchingResults.Count == 1)
            return RelativePath(matchingResults.ElementAt(0), pwd);
        else if (matchingResults.Count == 0)
            return "";
        else
        {
            Console.WriteLine();
            if (matchingResults.Count > elementsToRequest)
            {
                // TODO: print actual promtp instaead of "$ "
                Console.Write($"\n{matchingResults.Count} elements found. Show them all (y/n): ");
                ConsoleKeyInfo answer = Console.ReadKey();
                Console.WriteLine();
                if (answer.Key != ConsoleKey.Y)
                {
                    Console.Write("\n$ " + currentCommand.ToString());
                    return "";
                }
            }
            foreach (string element in matchingResults)
            {
                Console.Write($"{RelativePath(element, pwd)}\t");
            }
            Console.Write("\n$ " + currentCommand.ToString());
            return "";
        }
    }

    private static string RelativePath(string fullPath, string pwd) => fullPath[(pwd.Length + 1)..];
}