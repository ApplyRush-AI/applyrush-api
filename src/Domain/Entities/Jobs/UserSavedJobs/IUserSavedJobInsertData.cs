namespace Domain.Entities.Jobs.UserSavedJobs;

public interface IUserSavedJobInsertData
{
    int UserId { get; }
    int JobId { get; }
}
