using CommandExecution;
using BuiltInCommands;
using Aliases;
using CommandHistory;

namespace CommandProcessing;

public static class CommandProcessor
{
    private static AliasManager am = new();
    private static BuiltIn bi = new(am);
    private static readonly List<string> builtInCommands = new() { "cd", "exit", "set", "alias" };
    
    /// <summary>
    /// Get a list of commands that can be executed as they are
    /// ignores pipes
    /// </summary>
    /// <param name="fullCommand">the full line as it is typed in by the user</param>
    /// <returns>a list of commands ready to run as objects of the Command struct</returns> 
    public static List<Command> PreProcess(string fullCommand)
    {
        List<Command> commands = new();
        List<string> commandString = fullCommand.Trim(';').Split(';').ToList();
        foreach (string commandSplit in commandString)
        {
            string separatedCommand = commandSplit.Trim();
            // ignore comments
            if (separatedCommand.StartsWith("#") || separatedCommand.StartsWith("//"))
                break;

            // replace alias keywords with the actual commands
            separatedCommand = am.applyAliases(separatedCommand);

            // do not run empty commands
            if (separatedCommand.Trim() == string.Empty)
                return new();

            // apply !! operator
            int doubleExclamationMarkPos = separatedCommand.IndexOf("!!");
            if (doubleExclamationMarkPos > -1)
            {
                if (doubleExclamationMarkPos < separatedCommand.Length - 2)
                {
                    if (int.TryParse(new ReadOnlySpan<char>(separatedCommand[doubleExclamationMarkPos + 2]),
                            out int index))
                    {
                        separatedCommand = separatedCommand.Replace($"!!{index}", History.StoredCommand(index));
                    }
                } // No index given and !! at the end of the command
                else
                    separatedCommand = separatedCommand.Replace($"!!", History.StoredCommand(0));
            }
            Command command = split(separatedCommand);
            command.Arguments = processArguments(command);
            if (isBuiltIn(command))
            {
                bi.executeBuiltInCommand(command);
                continue; // built in commands should not be added to the list and therefore skipped
            }
            commands.Add(command);
        }
        return commands;
    }
    
    // split the command into program name and arguments
    public static Command split(string fullCommand)
    {
        int spacePosition = fullCommand.IndexOf(' ');
        if (spacePosition == -1)
            return new Command(fullCommand, "");
        else if (spacePosition == fullCommand.Length - 1)
            return new Command(fullCommand, "");

        string cmd = fullCommand[..spacePosition];
        string args = fullCommand[(spacePosition + 1)..];
        return new Command(cmd, args);
    }
    
    /// <summary>
    /// replace variables with their values and path modifiers like ~ with the full paths
    /// </summary>
    /// <param name="command"> The original command </param>
    /// <returns> the new arguments for the command </returns>
    private static string processArguments(Command command)
    {
        string args = String.Empty;
        List<string> split = Util.splitCommand(command.Arguments);

        // variables
        foreach (string arg in split)
        {
            if (arg == "")
                break;
            
            // two dollar signs can be used to give the range of a variable name
            if (arg.Contains('$'))
            {
                 
                int dsPosition = arg.IndexOf('$'); // can't be -1 because of the if statement above
                int secondDs = arg.Substring(dsPosition + 1).IndexOf('$') + dsPosition;
                string? value;
                if (secondDs == -1)
                    value = Environment.GetEnvironmentVariable(arg.Substring(dsPosition + 1));
                else
                    value = Environment.GetEnvironmentVariable(arg.Substring(dsPosition + 1, secondDs - dsPosition));
                string result = arg.Substring(0, dsPosition);
                if (value == null)
                {
                    Console.WriteLine("Value not found!");
                }
                else
                {
                    result += value;
                    if (secondDs > -1)
                        result += arg.Substring(secondDs + 2);
                    result = result.Replace(" ", null);
                    args += result; 
                }
            }
            else if (arg.StartsWith("~"))
            {
                args += arg;
                args = args.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            else if (arg.StartsWith("!!"))
            {
                args += arg;
                if (arg.Length == 2)
                    args = args.Replace("!!", History.StoredCommand(0));
                else
                {
                    if (int.TryParse(arg.Substring(2), out int index))
                    {
                        args = args.Replace("!!" + index, History.StoredCommand(index));
                    }
                }
            }
            else
            {
                args += $"{arg} ";
            }
        }
        return args;
    }

    // overload method used to process the prompt
    public static string processArguments(string command)
    {
        return processArguments(new Command("", command));
    }
    
    private static bool isBuiltIn(Command command)
    {
        foreach (string cmd in builtInCommands)
        {
            if (cmd == command.CommandName)
                return true;
        }
        return false;
    }
}
