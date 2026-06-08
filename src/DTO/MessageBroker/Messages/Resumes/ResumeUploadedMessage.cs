namespace DTO.MessageBroker.Messages.Resumes;

public sealed record ResumeUploadedMessage : MessageBase
{
    public int ResumeId { get; init; }
    public int UserId { get; init; }
}
