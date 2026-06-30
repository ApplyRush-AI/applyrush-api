using DTO.Enums.Job;

namespace Application.Common.Interfaces.Services;

public sealed class JobSyncStateData
{
    public JobSyncStatus Status { get; set; } = JobSyncStatus.Idle;
    public DateTime? LastSyncedAt { get; set; }
    public int JobsIndexed { get; set; }
    public int FailedPages { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IJobSyncState
{
    Task<JobSyncStateData> GetAsync(CancellationToken cancellationToken);
    Task SetAsync(JobSyncStateData state, CancellationToken cancellationToken);
}
