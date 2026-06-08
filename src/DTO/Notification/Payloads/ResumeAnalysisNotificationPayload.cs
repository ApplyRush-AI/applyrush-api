namespace DTO.Notification.Payloads;

public sealed record ResumeAnalysisNotificationPayload
{
    public int AnalysisId { get; init; }
}
