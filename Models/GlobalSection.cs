using System.Text.RegularExpressions;

namespace SlnMerge.Models;

public class GlobalSection(string name, List<string> blocks)
{
    public string Name { get; } = name;
    public List<string> Blocks { get; } = blocks;

    public static Dictionary<string, GlobalSection> ExtractGlobalSections(string[] lines)
    {
        var dict = new Dictionary<string, GlobalSection>(StringComparer.OrdinalIgnoreCase);
        var globalSectionPattern = new Regex(@"^\s*GlobalSection\((.*?)\)\s*=\s*(.*?)$", RegexOptions.Compiled);
        bool inGlobal = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().Equals("Global", StringComparison.OrdinalIgnoreCase))
            {
                inGlobal = true;
                continue;
            }
            if (inGlobal && globalSectionPattern.IsMatch(lines[i]))
            {
                var match = globalSectionPattern.Match(lines[i]);
                string sectionName = match.Groups[1].Value;
                var block = new List<string> { lines[i] };
                i++;
                while (i < lines.Length && !lines[i].Trim().StartsWith("EndGlobalSection", StringComparison.OrdinalIgnoreCase))
                {
                    block.Add(lines[i]);
                    i++;
                }
                if (i < lines.Length) block.Add(lines[i]);
                if (!dict.ContainsKey(sectionName)) dict[sectionName] = new GlobalSection(sectionName, new List<string>());
                dict[sectionName].Blocks.Add(string.Join(Environment.NewLine, block));
            }
        }
        return dict;
    }
}