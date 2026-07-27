using DTO.Enums.Profile.Education;

namespace Domain.Entities.Profiles.Educations;

public interface IEducationBaseData
{
    string School { get; }
    string? Major { get; }    
    DegreeType DegreeType { get; }
    decimal? Gpa { get; }
    GpaScale GpaScale { get; }
    DateOnly? StartDate { get; }
    DateOnly? EndDate { get; }
    bool IsCurrent { get; }
}
