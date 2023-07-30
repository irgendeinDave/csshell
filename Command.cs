using System.Diagnostics;
using CommandHistory;

namespace cmd;

struct Command
{
    public string CommandName;
    public string Arguments;

    public Command(string cmdName, string args)
    {
        CommandName = cmdName;
        Arguments = args;
    }
}

public class CommandRunner
{
    private readonly Dictionary<string, string> aliases = new();

    public void runLine(string fullCommand)
    {
        List<string> commands = fullCommand.Trim(';').Split(';').ToList();
        foreach (string command in commands)
        {
            run(command.Trim());
        }
    }

    private List<string> builtInCommands = new() { "cd", "exit", "set", "alias" };

    private void run(string fullCommand)
    {
        // ignore comments
        if (fullCommand.StartsWith("#") || fullCommand.StartsWith("//"))
            return;

        // replace alias keywords with the actual commands
        fullCommand = applyAliases(fullCommand);

        // do not run empty commands
        if (fullCommand.Trim() == string.Empty)
            return;

        // apply !! operator
        int doubleExclamationMarkPos = fullCommand.IndexOf("!!");
        if (doubleExclamationMarkPos > -1)
        {
            if (doubleExclamationMarkPos < fullCommand.Length - 2)
            {
                if (int.TryParse(new ReadOnlySpan<char>(fullCommand[doubleExclamationMarkPos + 2]), out int index))
                {
                    fullCommand = fullCommand.Replace($"!!{index}", History.StoredCommand(index));
                }
            } // No index given and !! at the end of the command
            else
                fullCommand = fullCommand.Replace($"!!", History.StoredCommand(0));
        }

        if (run(split(fullCommand)) == 0)
            History.Append(fullCommand);
    }

    /// <returns> the exit code of the command </returns>
    private int run(Command command)
    {
        command.Arguments = processArguments(command);

        if (isBuildIn(command))
        {
            executeBuiltInCommand(command);
            return 0; // assume the process was successful for now
        }

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command.CommandName,
                Arguments = command.Arguments,
                UseShellExecute = false
            };

            Process runningCommand = Process.Start(psi);
            runningCommand.WaitForExit();
            int exitCode = runningCommand.ExitCode;

            Environment.SetEnvironmentVariable("?", exitCode.ToString());
            return exitCode;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{command.CommandName} could not be executed: {e.Message}");
            Environment.SetEnvironmentVariable("?", "2");
        }

        return 1;
    }

    // split the command into program name and arguments
    private Command split(string fullCommand)
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

    private bool isBuildIn(Command command)
    {
        foreach (string cmd in builtInCommands)
        {
            if (cmd == command.CommandName)
                return true;
        }
        return false;
    }

    // run a built in command
    private void executeBuiltInCommand(Command command)
    {
        if (command.CommandName == "exit")
            Environment.Exit(0);
        else if (command.CommandName == "cd")
            try
            {
                if (command.Arguments == "")
                    Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                else
                    Directory.SetCurrentDirectory(command.Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.SetEnvironmentVariable("?", "2");
            }
        else if (command.CommandName == "set")
        {
            string[] args = command.Arguments.Split('=');
            if (args.Length != 2)
            {
                Console.WriteLine("Variable could not be set: no value given");
                Environment.SetEnvironmentVariable("?", "2");
                return;
            }

            Environment.SetEnvironmentVariable(args[0], args[1]);
        }
        else if (command.CommandName == "alias")
        {
            string[] split = command.Arguments.Split('=');
            if (split.Length != 2)
                return;
            if (aliases.ContainsKey(split[0]))
            {
                aliases[split[0]] = split[1];
                return;
            }

            aliases.Add(split[0].Trim(), split[1].Trim());
        }
    }

    /// <summary>
    /// replace variables with their values and path modifiers like ~ with the full paths
    /// </summary>
    /// <param name="command"> The original command </param>
    /// <returns> the new arguments for the command </returns>
    private string processArguments(Command command)
    {
        string args = String.Empty;
        string[] split = command.Arguments.Split(' ');

        // variables
        foreach (string arg in split)
        {
            if (arg == "")
                break;

            if (arg[0] == '$')
            {
                string? value = Environment.GetEnvironmentVariable(arg.Substring(1));
                if (value == null)
                {
                    Console.WriteLine($"Value for {arg} not found!");
                }
                else
                {
                    value = value.Replace(" ", null);
                    args += value; 
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

    /// <summary>
    /// aliases are applied for each word separated by a space
    /// </summary>
    /// <param name="fullCommand">the original command</param>
    /// <returns>the new command with replaced aliases</returns>
    private string applyAliases(string fullCommand)
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