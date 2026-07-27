using Domain.Entities.Profiles.Educations;
using DTO.Enums.Profile.Education;

namespace Application.Features.Resumes.Data;

internal sealed record ParsedEducationInsertData(
    int UserProfileId,
    string School,
    string? Major,
    DegreeType DegreeType,
    decimal? Gpa,
    GpaScale GpaScale,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool IsCurrent) : IEducationInsertData;
