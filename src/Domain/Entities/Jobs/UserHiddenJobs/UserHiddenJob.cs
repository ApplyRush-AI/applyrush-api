using Domain.Entities.Base;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.User;

namespace Domain.Entities.Jobs.UserHiddenJobs;

public sealed class UserHiddenJob : BaseEntity
{
    private UserHiddenJob() { }

    public int UserId { get; private set; }
    public int JobId { get; private set; }

    public ApplicationUser User { get; } = null!;
    public JobListing Job { get; } = null!;

    public static UserHiddenJob Create(IUserHiddenJobInsertData data) => new()
    {
        UserId = data.UserId,
        JobId = data.JobId
    };
}
