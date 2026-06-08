using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExperiences;
using DTO.Enums.Job;

namespace Worker.Consumers.Resumes.Data;

internal sealed record ParsedWorkExperienceInsertData(
    int UserProfileId,
    string JobTitle,
    string Company,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Summary,
    IReadOnlyList<string> Bullets) : IWorkExperienceInsertData
{
    JobType? IWorkExperienceBaseData.JobType => null;
}
