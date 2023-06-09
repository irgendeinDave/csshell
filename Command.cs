using System.Diagnostics;

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
    public List<String> builtInCommands = new List<String> { "cd", "exit", "set" };
    public void run(string fullCommand)
    {
        if (fullCommand.StartsWith("#") || fullCommand.StartsWith("//"))
            return;

        run(split(fullCommand));
    }

    private void run(Command command)
    {
        replaceArguments(command, out command);
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

        string cmd = fullCommand.Substring(0, spacePosition);
        string args = fullCommand.Substring(spacePosition + 1);
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

    // replace any variable with its value
    private void replaceArguments(Command command, out Command cmd)
    {
        string args = String.Empty;
        string[] split = command.Arguments.Split(' ');
        foreach (string arg in split)
        {
            if (arg[0] == '$')
            {
                args += Environment.GetEnvironmentVariable(arg.Substring(1));
            }
            else args += arg;
        }
        cmd = command;
        cmd.Arguments = args;
    }
}