namespace DTO.Profile;

public record ProfileCompletionBreakdown
{
    public bool Personal { get; init; }
    public bool Experience { get; init; }
    public bool Education { get; init; }
    public bool Skills { get; init; }
    public bool Eeo { get; init; }
}
