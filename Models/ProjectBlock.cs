using System.Text.RegularExpressions;

namespace SlnMerge.Models
{
    public class ProjectBlock(string guid, string name, string block)
    {
        public string Guid { get; } = guid;
        public string Name { get; } = name;
        public string Block { get; } = block;

        public static Dictionary<string, ProjectBlock> ExtractProjectBlocks(string[] lines)
        {
            var dict = new Dictionary<string, ProjectBlock>(StringComparer.OrdinalIgnoreCase);
            var projectPattern = new Regex(@"^Project\(\""(.*?)\""\)\s=\s\""(.*?)\""\,\s\""(.*?)\""\,\s\""(.*?)\""$", RegexOptions.Compiled);
            for (int i = 0; i < lines.Length; i++)
            {
                if (projectPattern.IsMatch(lines[i]))
                {
                    var block = new List<string> { lines[i] };
                    var match = projectPattern.Match(lines[i]);
                    string guid = match.Groups[4].Value;
                    string name = match.Groups[2].Value;
                    i++;
                    while (i < lines.Length && !lines[i].Trim().Equals("EndProject", StringComparison.OrdinalIgnoreCase))
                    {
                        block.Add(lines[i]);
                        i++;
                    }
                    if (i < lines.Length) block.Add(lines[i]);
                    dict[guid] = new ProjectBlock(guid, name, string.Join(Environment.NewLine, block));
                }
            }
            return dict;
        }
    }
}