using Application.Common.Interfaces;
using Application.Identity;
using Domain.Entities.RefreshTokens;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Authentication.Core;

internal static class RefreshTokenHelper
{
    internal static async Task<RefreshToken> CreateAndSaveAsync(
        string refreshTokenValue,
        int userId,
        IdentityConfig config,
        IDateTime dateTime,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        var deviceInfo = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var refreshToken = RefreshToken.Create(
            refreshTokenValue,
            userId,
            dateTime.Now.Add(config.RefreshTokenValidity),
            deviceInfo,
            ipAddress);

        refreshToken.UpdateLastUsed(dateTime.Now);

        await dbContext.RefreshToken.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }
}
