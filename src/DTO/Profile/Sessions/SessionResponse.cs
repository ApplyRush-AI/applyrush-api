namespace DTO.Profile.Sessions;

public record SessionResponse
{
    public string Token { get; init; } = null!;
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public DateTime ExpiryTime { get; init; }
}
