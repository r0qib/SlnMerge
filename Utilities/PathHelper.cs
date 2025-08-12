namespace SlnMerge.Utilities;

public static class PathHelper
{
    public static string GetProjectRoot()
    {
        var dir = AppContext.BaseDirectory;
        var d = new DirectoryInfo(dir);
        for (int i = 0; i < 3; i++)
            d = d.Parent ?? d;
        return d.FullName;
    }

    public static string ResolvePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "";
        if (Path.IsPathRooted(path)) return path;
        var root = GetProjectRoot();
        return Path.Combine(root, path);
    }
}