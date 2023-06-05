
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
                Console.Write($"{files.Count} elements found. Show them all (y/n): ");
                string? answer = Console.ReadLine();
                if (answer != "y")
                    return null;

            }
            foreach (string element in files)
            {
                Console.Write($"{element}   ");
            }
            return null;
        }
    }

    private string relativePath(string fullPath, string pwd)
    {
        return fullPath.Substring(pwd.Length + 1);
    }
}