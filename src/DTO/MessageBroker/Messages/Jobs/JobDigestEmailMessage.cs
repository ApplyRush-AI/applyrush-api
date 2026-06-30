namespace DTO.MessageBroker.Messages.Jobs;

public sealed record JobDigestEmailMessage : MessageBase
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public IReadOnlyList<JobDigestItem> Jobs { get; init; } = [];
}
