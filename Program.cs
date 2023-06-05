﻿using System.Diagnostics;

public class Program
{
    private static string prompt = "$ ";

    public static void Main(String[] args)
    {
        Console.WriteLine("Welcome to CsShell!");

        InputManager inputReader = new InputManager();

        Console.Write(prompt);

        while (true)
        {
            string? input = inputReader.readInput();
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

            // check if command is a built in command and do not run it if it is
            if (executeBuiltInCommands(input)) continue;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(programName);
                psi.Arguments = arguments;
                Console.WriteLine("running command");
                Process.Start(psi);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{programName} could not be executed: {e.Message}");
            }
            Console.Write(prompt);
        }
    }

    // Parse built-ins and returns if the command is a built in command
    private static bool executeBuiltInCommands(string input)
    {
        if (input == "exit")
        {
            Environment.Exit(0);
            return true; // this is not really necessary but the compiler insists on it
        }
        else if (substringUntilChar(input, ' ') == "cd")
        {
            Console.WriteLine(input.Substring(2, input.Length - 2));
            if (input.Substring(2, input.Length - 2) == "")
                Directory.SetCurrentDirectory(Environment.GetEnvironmentVariable("HOME"));
            else
                Directory.SetCurrentDirectory(input.Substring(3));
            return true;
        }
        else return false;
    }

    private static string substringUntilChar(string input, char end)
    {
        int endIndex = input.IndexOf(end);
        if (endIndex != -1)
            return input.Substring(0, endIndex);
        else return input;
    }
}