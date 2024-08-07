using System.Text;
using Autocompletion;

namespace InputReader;

public class InputManager
{
    private StringBuilder currentCommand = new();
    private readonly Autocomplete ac = new();

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
                applyAutocomplete();
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (Console.GetCursorPosition().Left > Settings.Prompt().Length)
                {
                    Console.Write("\b \b"); 
                    currentCommand.Length--;
                }
            }
            else
            {
                currentCommand.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    private void applyAutocomplete()
    {
        try
        {
            List<string> split = new(currentCommand.ToString().Split(' '));
            string lastElementInSplit = split.ElementAt(split.Count - 1);

            string autocomplete = ac.AutocompleteResult(lastElementInSplit, currentCommand.ToString());
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
                    Console.Write("-");
                    currentCommand.Append("-");
                }
                return;
            }
            // BUG: Replace will replace all instances of the last element
            int lastSpaceIndex = currentCommand.ToString().LastIndexOf(' ');
            currentCommand = new (currentCommand.ToString()[..lastSpaceIndex] + currentCommand.ToString()
                .Substring(lastSpaceIndex).Replace(lastElementInSplit, autocomplete));
            List<string> newCommandSplit = new(currentCommand.ToString().Split(' '));
            Console.Write(newCommandSplit.ElementAt(newCommandSplit.Count - 1).Substring(lastElementInSplit.Length));
        }
        catch
        {
            // ignored
        }
    }
}