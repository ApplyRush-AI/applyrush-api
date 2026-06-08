namespace DTO.User;

public record UserBaseResponse
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Picture { get; set; }
}
