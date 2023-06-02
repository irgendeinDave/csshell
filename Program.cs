using System.Diagnostics;
using System.Text;

public class Program
{
    private static string promt = "$ ";
    private static string startPath;
    public static void Main(String[] args)
    {

        Console.WriteLine("Welcome to CsShell!");
        while (true)
        {
            Console.Write(promt);
            string? input = Console.ReadLine();
            if (String.IsNullOrEmpty(input))
                continue;



            try
            {
                Process.Start(input);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{input} could not be executed: {e.Message}");
            }
        }
    }
}