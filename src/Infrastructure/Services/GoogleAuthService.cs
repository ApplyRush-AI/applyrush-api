using Application.Common.Interfaces.Auth.ExternalAuth;
using Application.Common.Localization;
using Google.Apis.Auth;

namespace Infrastructure.Services;

internal sealed class GoogleAuthService : IExternalAuthService
{
    private readonly ILocalizationService _localizationService;

    public GoogleAuthService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<ExternalAuthUserInfo> AuthenticateAsync(string idToken, CancellationToken cancellationToken = default)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.invalidGoogleToken.error.message"), ex);
        }

        var nameParts = (payload.Name ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : payload.GivenName ?? string.Empty;
        var lastName = nameParts.Length > 1
            ? string.Join(' ', nameParts.Skip(1))
            : payload.FamilyName ?? string.Empty;

        return new ExternalAuthUserInfo(payload.Email, firstName, lastName);
    }
}
