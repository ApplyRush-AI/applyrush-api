using DTO.Attributes;

namespace DTO.Enums.Job;

public enum JobSyncStatus
{
    [LocalizationKey("enum.jobSyncStatus.idle")]
    Idle = 1,
    [LocalizationKey("enum.jobSyncStatus.running")]
    Running = 2,
    [LocalizationKey("enum.jobSyncStatus.failed")]
    Failed = 3
}
