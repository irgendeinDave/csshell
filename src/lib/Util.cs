public static class Util
{
    public static string RelativePath(string fullPath, string pwd) => fullPath[(pwd.Length + 1)..];
    public static string RelativePathToHome(string fullPath)
    {
        if (UserHome.Length + 1 <= fullPath.Length)
            return "~/" + fullPath[(UserHome.Length + 1)..];
        else return "~";
    }

    public static readonly string UserHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}