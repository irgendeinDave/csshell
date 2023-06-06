namespace Autocompletion;

public class Autocomplete
{
    private int elementsToRequest;
    public int ElementsToRequest
    {
        set { elementsToRequest = value; }
        get { return elementsToRequest; }
    }

    private List<String> files = new List<string>();

    public bool moreThanOneElement()
    {
        files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).ToList<string>();
        return files.Count > 1;
    }

    ///<summary>
    /// scan for available files in the working directory
    /// if there are multiple offer to print out all available ones and return an empty string
    /// else return the file name
    ///</summary>
    public string autocompleteResult(string commandStart, string currentCommand)
    {
        string pwd = Directory.GetCurrentDirectory();
        files = Directory.EnumerateFiles(pwd).ToList<string>();
        List<String> directories = Directory.EnumerateDirectories(pwd).ToList<string>();
        List<String> results = files;
        results.AddRange(directories);
        List<String> matchingResults = new List<String>();
        foreach (string result in results)
        {
            if (relativePath(result, pwd).StartsWith(commandStart))
                matchingResults.Add(result);
        }
        if (matchingResults.Count == 1)
            return relativePath(matchingResults.ElementAt(0), pwd);
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
                    Console.Write("\n$ " + currentCommand.ToString());
                    return "";
                }
            }
            foreach (string element in matchingResults)
            {
                Console.Write($"{relativePath(element, pwd)}   ");
            }
            Console.Write("\n$ " + currentCommand.ToString());
            return "";
        }
    }

    private string relativePath(string fullPath, string pwd)
    {
        return fullPath.Substring(pwd.Length + 1);
    }
}