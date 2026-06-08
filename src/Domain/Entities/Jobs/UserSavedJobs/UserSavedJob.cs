using Domain.Entities.Base;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.User;
using DTO.Enums;

namespace Domain.Entities.Jobs.UserSavedJobs;

public sealed class UserSavedJob : BaseAuditableEntity
{
    private UserSavedJob() { }

    public int UserId { get; private set; }
    public int JobId { get; private set; }
    public Status Status { get; private set; }

    public ApplicationUser User { get; } = null!;
    public JobListing Job { get; } = null!;

    public static UserSavedJob Create(IUserSavedJobInsertData data)
    {
        return new UserSavedJob
        {
            UserId = data.UserId,
            JobId = data.JobId,
            Status = Status.Active
        };
    }

    public void Delete() 
    { 
        Status = Status.Deleted; 
    }
}
