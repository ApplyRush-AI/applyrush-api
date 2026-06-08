using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums;
using DTO.Enums.Profile.Education;

namespace Domain.Entities.Profiles.Educations;

public sealed class Education : BaseAuditableEntity, IWithStatus
{
    private Education() { }

    public int UserProfileId { get; private set; }
    public string School { get; private set; } = null!;
    public string? Major { get; private set; }
    public DegreeType DegreeType { get; private set; }
    public decimal? Gpa { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public int OrderIndex { get; private set; }
    public Status Status { get; private set; }

    public UserProfile UserProfile { get; } = null!;

    public static Education Create(IEducationInsertData data, int orderIndex)
    {
        return new Education
        {
            UserProfileId = data.UserProfileId,
            School = data.School,
            Major = data.Major,
            DegreeType = data.DegreeType,
            Gpa = data.Gpa,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            IsCurrent = data.IsCurrent,
            OrderIndex = orderIndex,
            Status = Status.Active
        };
    }

    public void Update(IEducationUpdateData data)
    {
        School = data.School;
        Major = data.Major;
        DegreeType = data.DegreeType;
        Gpa = data.Gpa;
        StartDate = data.StartDate;
        EndDate = data.EndDate;
        IsCurrent = data.IsCurrent;
    }

    public void SetOrderIndex(int orderIndex)
    {
        OrderIndex = orderIndex;
    }

    public void Activate() => Status = Status.Active;
    public void Deactivate() => Status = Status.Inactive;

    public void Delete() => Status = Status.Deleted;
}
