using cmd;
using InputReader;
using EnvironmentSetup;
using CommandHistory;

public class Program
{
    private static string prompt = "$ ";
    private static readonly CommandRunner cr = new();

    public static void Main(String[] args)
    {
        Setup.startSetup();
        Console.WriteLine("Welcome to CsShell!");
                Console.WriteLine("Debug: " + History.StoredCommand(0));

        if (args.Length == 1)
        {
            Console.WriteLine("DEBUG: " + args[0]);
            string path = args[0];
            Script.runScript(path, cr);
        }

        InputManager inputReader = new();

        while (true)
        {
            Console.Write(prompt);

            string? input = inputReader.readInput();
            if (String.IsNullOrEmpty(input))
                continue;

            cr.run(input);
        }
    }
}