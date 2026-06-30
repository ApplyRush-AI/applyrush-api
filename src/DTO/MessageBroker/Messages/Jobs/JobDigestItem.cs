namespace DTO.MessageBroker.Messages.Jobs;

public sealed record JobDigestItem
{
    public string Title { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string? Industry { get; init; }
    public string? Salary { get; init; }
    public string Location { get; init; } = null!;
    public int MatchScore { get; init; }
    public string PostedAgo { get; init; } = null!;
    public string ApplyUrl { get; init; } = null!;
}
