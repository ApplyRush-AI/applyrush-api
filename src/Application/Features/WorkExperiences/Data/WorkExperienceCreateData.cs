using Domain.Entities.Profiles.WorkExperiences;
using DTO.Enums.Job;

namespace Application.Features.WorkExperiences.Data;

public sealed record WorkExperienceCreateData(
    int UserProfileId,
    string JobTitle,
    string Company,
    JobType? JobType,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Summary
    ) : IWorkExperienceInsertData;
