using DTO.MessageBroker.Messages.Authenticate;

namespace DTO.Authentication;

public record LoginResponse
{
    public LoginUserInfo User { get; init; } = new LoginUserInfo();
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}