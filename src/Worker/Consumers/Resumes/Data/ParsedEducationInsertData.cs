using Domain.Entities.Profiles.Educations;
using DTO.Enums.Profile.Education;

namespace Worker.Consumers.Resumes.Data;

internal sealed record ParsedEducationInsertData(
    int UserProfileId,
    string School,
    string? Major,
    DegreeType DegreeType,
    decimal? Gpa,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool IsCurrent) : IEducationInsertData;
