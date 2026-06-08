namespace DTO.Authentication;

public record LoginUserInfo
{
    public int Id { get; init; }
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Initials { get; init; } = null!;
}
