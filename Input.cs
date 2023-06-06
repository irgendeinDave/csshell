using System.Text;
using Autocompletion;

namespace InputReader;

public class InputManager
{
    private StringBuilder currentCommand = new StringBuilder();
    private Autocomplete ac = new Autocomplete();
    private uint charactersInput;

    public InputManager()
    {
        ac.ElementsToRequest = 5;
    }

    public string readInput()
    {
        currentCommand.Clear();
        charactersInput = 0;
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
                applyAutocomplete();
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (charactersInput == 0)
                    continue;

                Console.Write("\b \b");
                charactersInput--;
                currentCommand.Length--;
            }
            else
            {
                charactersInput++;
                currentCommand.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    private void applyAutocomplete()
    {
        try
        {
            List<string> split = new List<string>(currentCommand.ToString().Split(' '));
            string lastElementInSplit = split.ElementAt(split.Count - 1);

            string autocomplete = ac.autocompleteResult(lastElementInSplit, currentCommand.ToString());
            // simply append autocomplete suggestion when command ends in a space character
            if (currentCommand.ToString()[currentCommand.ToString().Length - 1] == ' ')
            {
                Console.Write(autocomplete);
                currentCommand.Append(autocomplete);
                return;
            }

            if (lastElementInSplit[0] == '-')
            {
                if (lastElementInSplit.Length == 1)
                {
                    Console.WriteLine("-");
                    currentCommand.Append("-");
                }
                return;
            }
            currentCommand.Replace(lastElementInSplit, autocomplete);
            List<string> newCommandSplit = new List<string>(currentCommand.ToString().Split(' '));
            Console.Write(newCommandSplit.ElementAt(newCommandSplit.Count - 1).Substring(lastElementInSplit.Length));
        }
        catch { } // ignore 
    }
}