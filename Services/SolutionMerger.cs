using SlnMerge.Models;

namespace SlnMerge.Services;

public class SolutionMerger
{
    public static int Merge(string targetPath, string sourcePath, string outputPath)
    {
        var target = new SolutionParser(targetPath);
        var source = new SolutionParser(sourcePath);

        var nameToGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in source.Projects)
            nameToGuid[kvp.Value.Name] = kvp.Key;

        var guidsToRemove = new HashSet<string>();
        foreach (var kvp in target.Projects)
        {
            if (nameToGuid.TryGetValue(kvp.Value.Name, out var sourceGuid) && sourceGuid != kvp.Key)
                guidsToRemove.Add(kvp.Key);
        }

        var mergedProjects = new Dictionary<string, ProjectBlock>(target.Projects, StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in source.Projects)
        {
            if (!mergedProjects.ContainsKey(kvp.Key))
                mergedProjects[kvp.Key] = kvp.Value;
        }
        foreach (var guid in guidsToRemove)
            mergedProjects.Remove(guid);

        var guidMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in target.Projects)
        {
            if (nameToGuid.TryGetValue(kvp.Value.Name, out var sourceGuid) && sourceGuid != kvp.Key)
                guidMap[kvp.Key] = sourceGuid;
        }

        var mergedSections = new Dictionary<string, GlobalSection>(target.Sections, StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in source.Sections)
        {
            if (!mergedSections.ContainsKey(kvp.Key))
                mergedSections[kvp.Key] = new GlobalSection(kvp.Key, new List<string>());
            mergedSections[kvp.Key].Blocks.AddRange(kvp.Value.Blocks);
        }

        foreach (var section in mergedSections.Keys)
        {
            var updatedBlocks = new List<string>();
            foreach (var block in mergedSections[section].Blocks)
                updatedBlocks.Add(ReplaceGuids(block, guidMap));
            mergedSections[section] = new GlobalSection(section, updatedBlocks);
        }

        // Special handling for NestedProjects
        if (mergedSections.ContainsKey("NestedProjects"))
        {
            var allNested = new HashSet<string>();
            foreach (var block in mergedSections["NestedProjects"].Blocks)
            {
                foreach (var line in block.Split(Environment.NewLine))
                {
                    if (line.Contains("="))
                        allNested.Add(line.Trim());
                }
            }
            var nestedBlock = new List<string>
            {
                "\tGlobalSection(NestedProjects) = preSolution"
            };
            foreach (var entry in allNested)
                nestedBlock.Add("\t\t" + entry);
            nestedBlock.Add("\tEndGlobalSection");
            mergedSections["NestedProjects"] = new GlobalSection("NestedProjects", new List<string> { string.Join(Environment.NewLine, nestedBlock) });
        }

        // Compose output
        var output = new List<string>();
        bool insideProjectSection = false;
        for (int i = 0; i < target.Lines.Length; i++)
        {
            if (target.Lines[i].StartsWith("Project("))
            {
                if (!insideProjectSection)
                {
                    foreach (var block in mergedProjects.Values)
                        output.AddRange(block.Block.Split(Environment.NewLine));
                    insideProjectSection = true;
                }
                while (i < target.Lines.Length && !target.Lines[i].Trim().Equals("EndProject", StringComparison.OrdinalIgnoreCase))
                    i++;
            }
            else if (target.Lines[i].Trim().Equals("Global", StringComparison.OrdinalIgnoreCase))
            {
                output.Add(target.Lines[i]);
                foreach (var section in mergedSections)
                {
                    foreach (var block in section.Value.Blocks)
                        output.AddRange(block.Split(Environment.NewLine));
                }
                while (i < target.Lines.Length && !target.Lines[i].Trim().Equals("EndGlobal", StringComparison.OrdinalIgnoreCase)) i++;
            }
            else
            {
                output.Add(target.Lines[i]);
            }
        }

        File.WriteAllLines(outputPath, output);
        Console.WriteLine($"Merged solution written to {outputPath}");
        
        Console.WriteLine("Press any key to close...");
        Console.ReadKey();

        return 0;
    }

    private static string ReplaceGuids(string block, Dictionary<string, string> guidMap)
    {
        foreach (var kvp in guidMap)
            block = block.Replace(kvp.Key, kvp.Value);
        return block;
    }
}