using DTO.Enums.Job;

namespace Domain.Entities.Jobs.JobListings;

public interface IJobListingBaseData
{
    string Title { get; }
    string Company { get; }
    string? CompanyLogoUrl { get; }
    string Description { get; }
    string? About { get; }
    string? Responsibilities { get; }
    string? Requirements { get; }
    string? Benefits { get; }
    string? RequiredSkills { get; }
    string? Industry { get; }
    string Location { get; }
    WorkModel WorkModel { get; }
    JobType JobType { get; }
    ExperienceLevel ExperienceLevel { get; }
    decimal? SalaryMin { get; }
    decimal? SalaryMax { get; }
    string? Currency { get; }
    int? YearsRequired { get; }
    int? ApplicantCount { get; }
    DateTime PostedAt { get; }
    DateTime? ExpiresAt { get; }
    string ApplyUrl { get; }
    bool H1BSupported { get; }
    string? AiSummary { get; }
    DateTime LastSyncedAt { get; }
}
