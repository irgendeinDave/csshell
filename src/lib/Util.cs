public static class Util
{
    public static string RelativePath(string fullPath, string pwd) => fullPath[(pwd.Length + 1)..];
    public static string RelativePathToHome(string fullPath)
    {
        if (UserHome.Length + 1 <= fullPath.Length)
            return "~/" + fullPath[(UserHome.Length + 1)..];
        return "~";
    }

    public static readonly string UserHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static List<string> splitCommand(string command)
    {
        List<string> result = new();
        
        // split the command on the quotes and then split every part of that with even index

        var quoteSplit = command.Split('\"');
        for (int i = 0; i < quoteSplit.Length; ++i)
        {
            if (i % 2 == 1)
                result.AddRange(quoteSplit[i].Split(" "));
            else 
                result.Add(quoteSplit[i]);
        }
        return result;

    }
}