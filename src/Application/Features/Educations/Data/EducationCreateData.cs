using Domain.Entities.Profiles.Educations;
using DTO.Enums.Profile.Education;

namespace Application.Features.Educations.Data;

public sealed record EducationCreateData(
    int UserProfileId,
    string School,
    string? Major,
    DegreeType DegreeType,
    decimal? Gpa,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool IsCurrent
    ) : IEducationInsertData;
