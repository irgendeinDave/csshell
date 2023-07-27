using cmd;
using EnvironmentSetup;
using InputReader;

public class Program
{
    private static readonly string prompt = "$ ";
    private static readonly CommandRunner cr = new();

    public static void Main(string[] args)
    {
        Setup.startSetup();
        Console.WriteLine("Welcome to CsShell!");

        Script.runScript(Settings.rcFilePath, cr);

        if (args.Length == 1)
        {
            var path = args[0];
            Script.runScript(path, cr);
        }

        InputManager inputReader = new();

        while (true)
        {
            Console.Write(prompt);

            var input = inputReader.readInput();
            if (string.IsNullOrEmpty(input))
                continue;

            cr.runLine(input);
        }
    }
}