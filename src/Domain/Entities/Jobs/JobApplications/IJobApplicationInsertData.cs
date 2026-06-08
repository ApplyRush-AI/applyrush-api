namespace Domain.Entities.Jobs.JobApplications;

public interface IJobApplicationInsertData
{
    int UserId { get; }
    int JobId { get; }
}
