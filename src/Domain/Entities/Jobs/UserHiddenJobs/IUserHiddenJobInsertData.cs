namespace Domain.Entities.Jobs.UserHiddenJobs;

public interface IUserHiddenJobInsertData
{
    int UserId { get; }
    int JobId { get; }
}
