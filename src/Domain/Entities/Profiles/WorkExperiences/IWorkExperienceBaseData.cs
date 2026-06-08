using DTO.Enums.Job;

namespace Domain.Entities.Profiles.WorkExperiences;

public interface IWorkExperienceBaseData
{
    string JobTitle { get; }
    string Company { get; }
    JobType? JobType { get; }
    string? Location { get; }
    DateOnly StartDate { get; }
    DateOnly? EndDate { get; }
    bool IsCurrent { get; }
    string? Summary { get; }
}
