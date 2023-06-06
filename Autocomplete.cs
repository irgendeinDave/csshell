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
    public string autocompleteResult(string commandStart)
    {
        string pwd = Directory.GetCurrentDirectory();
        files = Directory.EnumerateFiles(pwd).ToList<string>();
        List<String> matchingResults = new List<String>();

        if (files.Count > elementsToRequest)
        {
            Console.Write($"\n{files.Count} elements found. Show them all (y/n): ");
            ConsoleKeyInfo answer = Console.ReadKey();
            Console.WriteLine();
            if (answer.Key != ConsoleKey.Y)
                return "";
        }
        foreach (string element in files)
        {
            Console.Write($"{relativePath(element, pwd)}   ");
        }
        Console.WriteLine();
        return "";
    }

    private string relativePath(string fullPath, string pwd)
    {
        return fullPath.Substring(pwd.Length + 1);
    }
}