using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Profiles.UserPreferredJobFunctions;
using DTO.Enums;

namespace Domain.Entities.JobFunctions;

public sealed class JobFunction : BaseAuditableEntity, IWithStatus
{
    private JobFunction() { }

    public string Name { get; private set; } = null!;
    public int? ParentId { get; private set; }
    public Status Status { get; private set; }

    public JobFunction? Parent { get; } = null;
    public ICollection<JobFunction> Children { get; } = new List<JobFunction>();
    public ICollection<UserPreferredJobFunction> UserPreferences { get; } = new List<UserPreferredJobFunction>();

    public static JobFunction Create(IJobFunctionUpsertData data)
    {
        return new JobFunction
        {
            Name = data.Name,
            ParentId = data.ParentId,
            Status = Status.Active
        };
    }

    public void Update(IJobFunctionUpsertData data)
    {
        Name = data.Name;
        ParentId = data.ParentId;
    }

    public void Activate() => Status = Status.Active;
    public void Deactivate() => Status = Status.Inactive;
}
