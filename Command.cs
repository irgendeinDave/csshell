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

    public void runLine(string fullCommand)
    {
        List<string> lines = fullCommand.Trim(';').Split(';').ToList();
        foreach (string line in lines)
        {
            run(line.Trim());
            Console.WriteLine();
        }
    }

    public List<string> builtInCommands = new () { "cd", "exit", "set" };
    private void run(string fullCommand)
    {
        // apply !! operator
        int doubleExclamationMarkPos = fullCommand.IndexOf("!!");
        if (doubleExclamationMarkPos > -1)
        {
            if (doubleExclamationMarkPos < fullCommand.Length - 2)
            {
                int index;
                if (int.TryParse(new ReadOnlySpan<char>(fullCommand[doubleExclamationMarkPos + 2]), out index))
                {
                    fullCommand = fullCommand.Replace($"!!{index}", History.StoredCommand(index));
                }
            } // No index given and !! at the end of the command
            else
                fullCommand = fullCommand.Replace($"!!", History.StoredCommand(0));
        }

        // ignore comments
        if (fullCommand.StartsWith("#") || fullCommand.StartsWith("//"))
            return;

        run(split(fullCommand));
        History.Append(fullCommand);
    }

    private void run(Command command)
    {
        command.Arguments = processArguments(command);

        if (isBuildIn(command))
        {
            executeBuiltInCommand(command);
            return;
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
        }
        catch (Exception e)
        {
            Console.WriteLine($"{command.CommandName} could not be executed: {e.Message}");
            Environment.SetEnvironmentVariable("?", "2");
        }
    }

    // split the command into program name and argument
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
                    Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("HOME"));
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
                
            else if (arg[0] == '$')
            {
                string value = Environment.GetEnvironmentVariable(arg.Substring(1));
                value = value.Replace(" ", null);
                args += value;
            }
            else if (arg.StartsWith("~"))
            {
                args += arg;
                args = args.Replace("~", Environment.GetEnvironmentVariable("HOME"));
            } 
            else if (arg.StartsWith("!!"))
            {
                args += arg;
                if (arg.Length == 2)
                    args = args.Replace("!!", History.StoredCommand(0));
                else 
                {
                    int index;
                    if (int.TryParse(arg.Substring(2), out index))
                    {
                        args = args.Replace("!!" + index, History.StoredCommand(index));
                    }
                }
            }
            // TODO: replace !! with last command in history      
            else 
            {
                args += $"{arg} ";      
            }
        }
        return args;
    }
/* 
    private static Command applyLastCommandToCommandName(Command command)
    {
        Command cmd = command;
        string loadedCommand = string.Empty;
        if (cmd.CommandName.StartsWith("!!"))
        {
            if (cmd.CommandName.Length == 2)
                loadedCommand = History.StoredCommand(0);
            else   
                if (int.TryParse(cmd.CommandName.Substring(2), out int index))                
                    loadedCommand = History.StoredCommand(index);          
        }
        if (!loadedCommand.Contains(' ', StringComparison.CurrentCulture))
        {
            cmd.CommandName = loadedCommand;
            cmd.Arguments = string.Empty;
        }
        else {
            cmd.CommandName = loadedCommand[..(loadedCommand.IndexOf(' ') - 1)];
            cmd.Arguments = loadedCommand[loadedCommand.IndexOf(' ')..];

        }
        return cmd;
    }*/
}
 