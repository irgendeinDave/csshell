using System.Text;
using Autocompletion;

namespace InputReader;

public class InputManager
{
    private readonly Autocomplete ac = new();
    private readonly StringBuilder currentCommand = new();
    private readonly string prompt = "$ ";

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
            else if (keyInfo.Key == ConsoleKey.Backspace && Console.GetCursorPosition().Left > prompt.Length)
            {
                Console.Write("\b \b");
                currentCommand.Length--;
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
            var lastElementInSplit = split.ElementAt(split.Count - 1);

            var autocomplete = ac.AutocompleteResult(lastElementInSplit, currentCommand.ToString());
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

            currentCommand.Replace(lastElementInSplit, autocomplete);
            List<string> newCommandSplit = new(currentCommand.ToString().Split(' '));
            Console.Write(newCommandSplit.ElementAt(newCommandSplit.Count - 1).Substring(lastElementInSplit.Length));
        }
        catch
        {
        } // ignore 
    }
}