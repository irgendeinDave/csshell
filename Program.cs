using System.Diagnostics;

public class Program
{
    public static void Main(String[] args)
    {
        Console.WriteLine("Welcome to PowerlessShell!");
        while (true)
        {
            Process.Start(Console.ReadLine());
        }
    }
}