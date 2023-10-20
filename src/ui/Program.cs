using CommandExecution;
using InputReader;
using EnvironmentSetup;

public class Program
{
    private static readonly CommandRunner cr = new();

    public static void Main(String[] args)
    {
        Setup.startSetup();
        Console.WriteLine("Welcome to CsShell!");

        Script.runScript(Settings.RcFilePath, cr);

        if (args.Length == 1)
        {
            string path = args[0];
            Script.runScript(path, cr);
        }

        InputManager inputReader = new();

        while (true)
        {
            Console.Write(Settings.Prompt());

            string input = inputReader.readInput();
            if (string.IsNullOrEmpty(input))
                continue;

            cr.runLine(input);
        }
    }
}