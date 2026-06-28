namespace DTO.Extension;

public sealed class ExtensionCreditResponse
{
    public int Used { get; init; }
    public int Remaining { get; init; }
    public int Total { get; init; }
    public DateTime ResetsAt { get; init; }
}
