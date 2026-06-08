using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using Domain.Entities.Profiles.WorkExperiences;
using DTO.Enums;
using DTO.Enums.Job;

namespace Domain.Entities.Profiles.WorkExpeciences;

public sealed class WorkExperience : BaseAuditableEntity, IWithStatus
{
    private WorkExperience() { }

    public int UserProfileId { get; private set; }
    public string JobTitle { get; private set; } = null!;
    public string Company { get; private set; } = null!;
    public JobType? JobType { get; private set; }
    public string? Location { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Summary { get; private set; }
    public int OrderIndex { get; private set; }
    public Status Status { get; private set; }

    public UserProfile UserProfile { get; } = null!;
    public ICollection<WorkExperienceBullet> Bullets { get; } = new List<WorkExperienceBullet>();

    public static WorkExperience Create(IWorkExperienceInsertData data, int orderIndex)
    {
        return new WorkExperience
        {
            UserProfileId = data.UserProfileId,
            JobTitle = data.JobTitle,
            Company = data.Company,
            JobType = data.JobType,
            Location = data.Location,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            IsCurrent = data.IsCurrent,
            Summary = data.Summary,
            OrderIndex = orderIndex,
            Status = Status.Active
        };
    }

    public void Update(IWorkExperienceUpdateData data)
    {
        JobTitle = data.JobTitle;
        Company = data.Company;
        JobType = data.JobType;
        Location = data.Location;
        StartDate = data.StartDate;
        EndDate = data.EndDate;
        IsCurrent = data.IsCurrent;
        Summary = data.Summary;
    }

    public void SetOrderIndex(int orderIndex)
    {
        OrderIndex = orderIndex;
    }

    public void Activate() => Status = Status.Active;
    public void Deactivate() => Status = Status.Inactive;

    public void Delete() => Status = Status.Deleted;
}
