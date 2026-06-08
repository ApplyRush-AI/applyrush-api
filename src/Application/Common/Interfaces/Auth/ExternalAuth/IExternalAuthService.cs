namespace Application.Common.Interfaces.Auth.ExternalAuth;

public interface IExternalAuthService
{
    Task<ExternalAuthUserInfo> AuthenticateAsync(string idToken, CancellationToken cancellationToken = default);
}
