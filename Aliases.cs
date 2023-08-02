
namespace Aliases;

public class AliasManager
{
    public Dictionary<string, string> aliases { get; } = new();

    /// <summary>
    /// aliases are applied for each word separated by a space
    /// </summary>
    /// <param name="fullCommand">the original command</param>
    /// <returns>the new command with replaced aliases</returns>
    public string applyAliases(string fullCommand)
    {
        string result = fullCommand;
        foreach (string arg in fullCommand.Split(' '))
        {
            foreach (KeyValuePair<string, string> kvp in aliases)
            {
                if (arg == kvp.Key)
                    result = result.Replace(kvp.Key, kvp.Value);
            }
        }

        return result;
    }
}