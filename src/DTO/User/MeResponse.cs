namespace DTO.User;

public sealed record MeResponse : UserInfoResponse
{
    public string? ProfilePicture { get; set; }
    public bool HasResume { get; set; }
}
