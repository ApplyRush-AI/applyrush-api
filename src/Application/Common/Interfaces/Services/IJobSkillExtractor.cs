namespace Application.Common.Interfaces.Services;

public interface IJobSkillExtractor
{
    // Returns the distinct skills a job asks for: the explicitly listed required skills unioned with the
    // skills from the curated dictionary that appear in the job's free-text (description, requirements, etc.).
    // Used for the resume gap analysis when a job listing carries no structured RequiredSkills (the common case).
    IReadOnlyList<string> ExtractSkills(string? jobText, IEnumerable<string> explicitSkills);
}
