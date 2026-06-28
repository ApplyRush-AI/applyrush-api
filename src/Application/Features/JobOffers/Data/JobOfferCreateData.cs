using Domain.Entities.Jobs.JobListings;
using DTO.Enums.Job;

namespace Application.Features.JobOffers.Data;

internal sealed record JobOfferCreateData(
    string ExternalId,
    JobSource Source,
    string Title,
    string Company,
    string? CompanyLogoUrl,
    string Description,
    string? About,
    string? Responsibilities,
    string? Requirements,
    string? Benefits,
    string? RequiredSkills,
    string? Industry,
    string Location,
    WorkModel WorkModel,
    JobType JobType,
    ExperienceLevel ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Currency,
    int? YearsRequired,
    int? ApplicantCount,
    DateTime PostedAt,
    DateTime? ExpiresAt,
    string ApplyUrl,
    bool H1BSupported,
    string? AiSummary,
    DateTime LastSyncedAt
) : IJobListingInsertData;
