

namespace EnvironmentSetup;

public  static class Setup
{
    public static void startSetup()
    {
        string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
        setupHistoryFile(userHomeDirectory);
        setupRcFile(userHomeDirectory);
    }

    private static  void setupHistoryFile(string homePath)
    {
        if (!File.Exists(homePath + "/.csshellhist"))
            File.Create(homePath + "/.csshellhist");
    }

    private static void setupRcFile(string homePath)
    {
        if (!File.Exists(homePath + "/.csshellrc"))
            File.Create(homePath + "/.csshellrc");
    }

}