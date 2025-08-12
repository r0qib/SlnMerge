using SlnMerge.Models;

namespace SlnMerge.Services;

public class SolutionParser
{
    public string[] Lines { get; }
    public Dictionary<string, ProjectBlock> Projects { get; }
    public Dictionary<string, GlobalSection> Sections { get; }

    public SolutionParser(string path)
    {
        Lines = File.ReadAllLines(path);
        Projects = ProjectBlock.ExtractProjectBlocks(Lines);
        Sections = GlobalSection.ExtractGlobalSections(Lines);
    }
}