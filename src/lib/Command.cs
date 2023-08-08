using System.Diagnostics;
using Aliases;
using CommandHistory;
using BuiltInCommands;

namespace CommandExecution;

public struct Command
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
    private static AliasManager am = new();
    private BuiltIn bi = new(am);
    public void runLine(string fullCommand)
    {
        List<string> commands = fullCommand.Trim(';').Split(';').ToList();
        foreach (string command in commands)
        {
            run(command.Trim());
        }
    }

    private readonly List<string> builtInCommands = new() { "cd", "exit", "set", "alias" };

    private void run(string fullCommand)
    {
        // ignore comments
        if (fullCommand.StartsWith("#") || fullCommand.StartsWith("//"))
            return;

        // replace alias keywords with the actual commands
        fullCommand = am.applyAliases(fullCommand);

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

        Command command = split(fullCommand);
        command.Arguments = processArguments(command);
        if (isBuildIn(command))
        {
            bi.executeBuiltInCommand(command);
            return;
        }

        int exitCode = execute(command);
        Environment.SetEnvironmentVariable("?", exitCode.ToString());

        if (exitCode == 0)
            History.Append(fullCommand);
    }

    private int execute(Command command)
    {
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
            return runningCommand.ExitCode;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{command.CommandName} could not be executed: {e.Message}");
            return 2;
        }
    }

    private int execute(Command command, out string stdOutput, out string stdError)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command.CommandName,
                Arguments = command.Arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process runningCommand = Process.Start(psi);
            runningCommand.WaitForExit();
            stdOutput = runningCommand.StandardOutput.ReadToEnd();
            stdError = runningCommand.StandardError.ReadToEnd();
            return runningCommand.ExitCode;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{command.CommandName} could not be executed: {e.Message}");
            stdError = string.Empty;
            stdOutput = String.Empty;
            return 2;
        }
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
}