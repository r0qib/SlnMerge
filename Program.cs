using System;
using SlnMerge.Utilities;
using SlnMerge.Services;

class Program
{
    static int Main(string[] args)
    {
        string? sourcePath = null;
        string? targetPath = null;
        string? outputPath = null;

        foreach (var arg in args)
        {
            if (arg == "-h" || arg == "--help")
            {
                PrintUsage();
                return 0;
            }
            if (arg.StartsWith("--source="))
                sourcePath = arg.Substring("--source=".Length);
            else if (arg.StartsWith("--target="))
                targetPath = arg.Substring("--target=".Length);
            else if (arg.StartsWith("--output="))
                outputPath = arg.Substring("--output=".Length);
        }

        sourcePath ??= Environment.GetEnvironmentVariable("SOURCE_SOLUTION");
        targetPath ??= Environment.GetEnvironmentVariable("TARGET_SOLUTION");
        outputPath ??= Environment.GetEnvironmentVariable("OUTPUT_SOLUTION");

        sourcePath = PathHelper.ResolvePath(sourcePath);
        targetPath = PathHelper.ResolvePath(targetPath);
        outputPath = PathHelper.ResolvePath(outputPath);

        if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(targetPath) || string.IsNullOrWhiteSpace(outputPath))
        {
            PrintUsage();
            return 1;
        }

        if (!System.IO.File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Source file does not exist: {sourcePath}");
            return 1;
        }
        if (!System.IO.File.Exists(targetPath))
        {
            Console.Error.WriteLine($"Target file does not exist: {targetPath}");
            return 1;
        }

        return SolutionMerger.Merge(targetPath, sourcePath, outputPath);
    }

    static void PrintUsage()
    {
        Console.WriteLine(
            "Usage:\n" +
            "  SlnMerge --source=path --target=path --output=path\n" +
            "Arguments:\n" +
            "  --source   Path to source .sln file\n" +
            "  --target   Path to target .sln file\n" +
            "  --output   Path for merged output .sln file\n" +
            "You may also set SOURCE_SOLUTION, TARGET_SOLUTION, OUTPUT_SOLUTION environment variables.\n" +
            "Use -h or --help to show this message."
        );
    }
}
