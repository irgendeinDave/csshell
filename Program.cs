using System.Diagnostics;
using System.Text;

public class Program
{
    private static string promt = "$ ";
    public static void Main(String[] args)
    {

        Console.WriteLine("vars: " + Environment.GetEnvironmentVariables());
        foreach (var x in Environment.GetEnvironmentVariables())
        {
            Console.WriteLine(x);
        }

        Console.WriteLine("Welcome to CsShell!");
        while (true)
        {
            Console.Write(promt);
            string? input = Console.ReadLine();
            if (String.IsNullOrEmpty(input))
                continue;

            // split input to get arguments
            int firstSpace = input.IndexOf(' ');
            string programName;
            string arguments = "";
            if (firstSpace != -1)
            {
                arguments = input.Substring(firstSpace, input.Length - firstSpace);
                programName = input.Substring(0, firstSpace);
            }
            else
            {
                arguments = "";
                programName = input;
            }

            #region built in commands
            executeBuiltInCommands(input);
            #endregion

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(programName);
                psi.Arguments = arguments;
                Process.Start(psi);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{programName} could not be executed: {e.Message}");
            }
        }
    }

    private static void executeBuiltInCommands(string input)
    {
        if (input == "exit")
            Environment.Exit(0);
    }
}