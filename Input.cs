using System.Text;

public class InputManager
{
    private StringBuilder currentCommand = new StringBuilder();
    private Autocomplete ac = new Autocomplete();
    struct Input
    {
        string command;
        string arguments;
    }
    private Input input = new Input();

    public InputManager()
    {
        ac.ElementsToRequest = 5;
    }

    public string readInput()
    {
        currentCommand.Clear();
        ConsoleKeyInfo keyInfo;
        while (true)
        {
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return currentCommand.ToString();
            }
            if (keyInfo.Key == ConsoleKey.Tab)
            {
                List<string> split = new List<string>(currentCommand.ToString().Split(' '));
                // if next argument hasn't started we have to append a new object to the end of the list
                if (currentCommand.ToString()[currentCommand.ToString().Length - 1] == ' ')
                    split.Add(" ");
                string lastElementInSplit = split.ElementAt(split.Count - 1);
                currentCommand.Replace(lastElementInSplit, ac.autocompleteResult(Directory.GetCurrentDirectory() + "/test"));
#warning DEBUG 
                List<string> newCommandSplit = new List<string>(currentCommand.ToString().Split(' '));
                Console.Write(newCommandSplit.ElementAt(newCommandSplit.Count - 1).Substring(lastElementInSplit.Length));
            }
            else
            {
                currentCommand.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }

        }
    }
}