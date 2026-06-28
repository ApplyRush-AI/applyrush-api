namespace DTO.Extension;

public sealed class ExtensionAutofillResponse
{
    public int SessionId { get; init; }
    public ExtensionProfileResponse Profile { get; init; } = null!;
    public int CreditsRemaining { get; init; }
}
