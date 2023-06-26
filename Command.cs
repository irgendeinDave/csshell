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
    public List<String> builtInCommands = new () { "cd", "exit", "set" };
    public void run(string fullCommand)
    {
        // ignore commands
        if (fullCommand.StartsWith("#") || fullCommand.StartsWith("//"))
            return;
        
        History.Append(fullCommand);

        run(split(fullCommand));
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
            ProcessStartInfo psi = new ProcessStartInfo(command.CommandName);
            psi.Arguments = command.Arguments;
            Process runningCommand = Process.Start(psi);
            runningCommand.WaitForExit();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{command.CommandName} could not be executed: {e.Message}");
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
            }
        else if (command.CommandName == "set")
        {
            string[] args = command.Arguments.Split('=');
            if (args.Length != 2)
            {
                Console.WriteLine("Variable could not be set: no value given");
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
            // TODO: replace !! with last command in history      
            else 
            {
                args += $"{arg} ";      
            }
        }
        return args;
    }
}