namespace DTO.Admin;

public sealed record AdminJobSyncStatusResponse(
    string Status,
    DateTime? LastSyncedAt,
    int JobsIndexed,
    string? ErrorMessage
);
