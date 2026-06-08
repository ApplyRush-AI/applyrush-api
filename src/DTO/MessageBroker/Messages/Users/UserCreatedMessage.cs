namespace DTO.MessageBroker.Messages.Users;

public sealed record UserCreatedMessage : MessageBase
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string Email { get; init; } = null!;
    public string EmailVerificationCode { get; init; } = null!;
    public Guid Uid { get; init; }
}
