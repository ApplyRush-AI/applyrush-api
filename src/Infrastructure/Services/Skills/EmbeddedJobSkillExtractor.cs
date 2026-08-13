using Application.Common.Interfaces.Services;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.Skills;

// Matches a curated skills dictionary against a job's free-text. The dictionary is embedded (skills-dictionary.json)
// and each entry is pre-compiled into a boundary-aware, case-insensitive regex once at startup, so the special
// characters common in tech skills (C#, .NET, Node.js) match correctly without partial-word false positives.
internal sealed class EmbeddedJobSkillExtractor : IJobSkillExtractor
{
    private readonly IReadOnlyList<(string Skill, Regex Pattern)> _dictionary;

    public EmbeddedJobSkillExtractor()
    {
        var assembly = typeof(EmbeddedJobSkillExtractor).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("skills-dictionary.json", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Embedded resource skills-dictionary.json not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        var skills = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stream)
            ?? throw new InvalidOperationException("Failed to deserialize skills-dictionary.json.");

        _dictionary = skills
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .DistinctBy(s => s.ToLowerInvariant())
            .Select(s => (s, BuildPattern(s)))
            .ToList();
    }

    public IReadOnlyList<string> ExtractSkills(string? jobText, IEnumerable<string> explicitSkills)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Explicit RequiredSkills win first and keep their original casing.
        foreach (var skill in explicitSkills)
        {
            var trimmed = skill?.Trim();
            if (!string.IsNullOrEmpty(trimmed) && seen.Add(trimmed))
                result.Add(trimmed);
        }

        if (!string.IsNullOrWhiteSpace(jobText))
        {
            foreach (var (skill, pattern) in _dictionary)
            {
                if (seen.Contains(skill)) continue;
                if (pattern.IsMatch(jobText) && seen.Add(skill))
                    result.Add(skill);
            }
        }

        return result;
    }

    // Boundaries are "not a letter or digit" so "C#", ".NET" and "Node.js" match, while "Java" does not match
    // inside "JavaScript". The leading boundary also excludes '#', '+' and '.' so "C" won't match inside "C#".
    private static Regex BuildPattern(string skill) =>
        new($@"(?<![A-Za-z0-9#+.]){Regex.Escape(skill)}(?![A-Za-z0-9#+.])",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
