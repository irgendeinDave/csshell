using System.Text;
using Autocompletion;

namespace InputReader;

public class InputManager
{
    private StringBuilder currentCommand = new StringBuilder();
    private Autocomplete ac = new Autocomplete();

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

                if (!ac.moreThanOneElement())
                {
                    Console.Write(currentCommand.ToString());
                    continue;
                }

                try
                {
                    List<string> split = new List<string>(currentCommand.ToString().Split(' '));
                    string lastElementInSplit = split.ElementAt(split.Count - 1);
                    Console.WriteLine(lastElementInSplit);

                    string autocomplete = ac.autocompleteResult(lastElementInSplit);
                    // simply append autocomplete suggestion when command ends in a space character
                    if (currentCommand.ToString()[currentCommand.ToString().Length - 1] == ' ')
                    {
                        Console.Write(autocomplete);
                        currentCommand.Append(autocomplete);
                        continue;
                    }

                    if (lastElementInSplit[0] == '-')
                        continue;
                    currentCommand.Replace(lastElementInSplit, autocomplete);
                    List<string> newCommandSplit = new List<string>(currentCommand.ToString().Split(' '));
                    Console.Write(newCommandSplit.ElementAt(newCommandSplit.Count - 1).Substring(lastElementInSplit.Length));
                }
                catch (Exception e)
                {
                    // ignore
                }
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                Console.Write("\b \b");
                if (currentCommand.Length > 0)
                    currentCommand.Length--;
            }
            else
            {
                currentCommand.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }
}