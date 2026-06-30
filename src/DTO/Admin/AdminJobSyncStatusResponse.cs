namespace DTO.Admin;

public sealed record AdminJobSyncStatusResponse(
    string Status,
    DateTime? LastSyncedAt,
    int JobsIndexed,
    int FailedPages,
    string? ErrorMessage
);
