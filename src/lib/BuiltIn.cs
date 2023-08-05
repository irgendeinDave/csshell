using CommandExecution;
using Aliases;
namespace BuiltInCommands;


public class BuiltIn
{
    private AliasManager am;
    public BuiltIn(AliasManager _am)
    {
        am = _am;
    }
    public void executeBuiltInCommand(Command command)
    {
        if (command.CommandName == "exit")
            Environment.Exit(0);
        else if (command.CommandName == "cd")
            try
            {
                if (command.Arguments == "")
                    Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                else
                    Directory.SetCurrentDirectory(command.Arguments.Trim());
                Environment.SetEnvironmentVariable("RELPWD", Util.RelativePathToHome(Directory.GetCurrentDirectory()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.SetEnvironmentVariable("?", "2");
            }
        else if (command.CommandName == "set")
        {
            string[] args = command.Arguments.Split('=', 2);
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
            string[] split = command.Arguments.Split('=', 2);
            if (split.Length != 2)
                return;
            if (am.aliases.ContainsKey(split[0]))
            {
                am.aliases[split[0]] = split[1];
                return;
            }
            am.aliases.Add(split[0].Trim(), split[1].Trim());
        }
    }
}