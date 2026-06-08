namespace DTO.Profile;

public record ProfileCompletionResponse
{
    public int Percentage { get; init; }
    public ProfileCompletionBreakdown Breakdown { get; init; } = null!;
}
