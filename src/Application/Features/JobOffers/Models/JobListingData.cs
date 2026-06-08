using Domain.Entities.Jobs.JobListings;
using DTO.Enums.Job;

namespace Application.Features.JobOffers.Models;

public sealed record JobListingData : IJobListingInsertData, IJobListingUpdateData
{
    public string ExternalId { get; init; } = null!;
    public JobSource Source { get; init; }
    public string Title { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string? CompanyLogoUrl { get; init; }
    public string Description { get; init; } = null!;
    public string? About { get; init; }
    public string? Responsibilities { get; init; }
    public string? Requirements { get; init; }
    public string? Benefits { get; init; }
    public string? RequiredSkills { get; init; }
    public string? Industry { get; init; }
    public string Location { get; init; } = null!;
    public WorkModel WorkModel { get; init; }
    public JobType JobType { get; init; }
    public ExperienceLevel ExperienceLevel { get; init; }
    public decimal? SalaryMin { get; init; }
    public decimal? SalaryMax { get; init; }
    public string? Currency { get; init; }
    public int? YearsRequired { get; init; }
    public int? ApplicantCount { get; init; }
    public DateTime PostedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string ApplyUrl { get; init; } = null!;
    public bool H1BSupported { get; init; }
    public string? AiSummary { get; init; }
    public DateTime LastSyncedAt { get; init; }
}
