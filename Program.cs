using cmd;
using InputReader;

public class Program
{
    private static string prompt = "$ ";
    private static readonly CommandRunner cr = new();

    public static void Main(String[] args)
    {
        Console.WriteLine("Welcome to CsShell!");
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