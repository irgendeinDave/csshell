using System.Diagnostics;
using CommandHistory;
using CommandProcessing;

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
    public void runLine(string fullCommand)
    {
        List<Command> processedCommand = CommandProcessor.PreProcess(fullCommand);
        foreach (Command command in processedCommand)
        {
            string combined = $"{command.CommandName} {command.Arguments}";
            int exitCode;
            // pipes
            if (fullCommand.Contains('|'))
            {
                string firstCommand = combined[..combined.IndexOf('|')].Trim();
                string secondCommand = combined[(combined.IndexOf('|') + 1)..].Trim();

                Command newcommand = CommandProcessor.split(firstCommand);

                exitCode = execute(newcommand, out string output, out string error);
                if (error != string.Empty)
                {
                    Console.Write(error);
                    Environment.SetEnvironmentVariable("?", exitCode.ToString());
                    continue;
                }

                exitCode = execute(CommandProcessor.split($"{secondCommand} {output}"));
                Environment.SetEnvironmentVariable("?", exitCode.ToString());
                if (exitCode == 0)
                    History.Append(combined);
                continue;
            }

            exitCode = execute(command);
            if (exitCode == 0)
                History.Append(combined);
        }
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
            return 1;
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
            stdError = $"{command.CommandName} could not be executed: {e.Message}";
            stdOutput = String.Empty;
            return 1;
        }
    }
}




    

    

    