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
    public List<String> builtInCommands = new List<String> { "cd", "exit" };
    public void run(string fullCommand)
    {
        run(split(fullCommand));
    }

    private void run(Command command)
    {
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
    }
}