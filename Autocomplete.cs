
public class Autocomplete
{
    private int elementsToRequest;
    public int ElementsToRequest
    {
        set { elementsToRequest = value; }
        get { return elementsToRequest; }
    }

    ///<summary>
    /// scan for available files in the working directory
    /// if there are multiple print out all available ones and return null
    /// else return the file name
    ///</summary>
    public string? autocompleteResult(string pwd)
    {
        List<string> files = Directory.EnumerateFiles(pwd).ToList<string>();
        if (files.Count == 1)
            return relativePath(files.ElementAt(0), pwd);
        else if (files.Count == 0)
            return "";
        else
        {
            if (files.Count > elementsToRequest)
            {
                Console.Write($"\n{files.Count} elements found. Show them all (y/n): ");
                ConsoleKeyInfo answer = Console.ReadKey();
                Console.WriteLine();
                if (answer.Key != ConsoleKey.Y)
                    return null;
            }
            foreach (string element in files)
            {
                Console.Write($"{relativePath(element, pwd)}   ");
            }
            Console.WriteLine();
            return null;
        }
    }

    private string relativePath(string fullPath, string pwd)
    {
        return fullPath.Substring(pwd.Length + 1);
    }
}