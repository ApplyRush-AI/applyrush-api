using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Authentication.Core;
using Application.Identity;
using AutoMapper;
using Domain.Entities.RefreshTokens;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Authentication.Commands.TokenRefresh;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<LoginResponse>
{
}

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDateTime _dateTime;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityConfig _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        UserManager<ApplicationUser> userManager,
        IDateTime dateTime,
        IOptions<IdentityConfig> config,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
        _dateTime = dateTime;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _config = config.Value;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await ResolveUserFromAccessTokenAsync(request.AccessToken);
        var currentRefreshToken = await ValidateRefreshTokenAsync(request.RefreshToken, user.Id);

        var (accessToken, _, newRefreshToken) = await _jwtTokenService.CreateAsync(user);

        await ReplaceRefreshTokenAsync(currentRefreshToken, newRefreshToken, user.Id, cancellationToken);

        return BuildLoginResponse(user, accessToken, newRefreshToken);
    }

    private async Task<ApplicationUser> ResolveUserFromAccessTokenAsync(string accessToken)
    {
        string username;
        try
        {
            username = _jwtTokenService.GetClaimFromToken(accessToken, "userName", false)!;
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException("Invalid access token");
        }

        var user = await _userManager.FindByNameAsync(username);
        return user ?? throw new UnauthorizedAccessException("Invalid access token");
    }

    private async Task<RefreshToken> ValidateRefreshTokenAsync(string refreshTokenValue, int userId)
    {
        var token = await _dbContext.RefreshToken
            .FirstOrDefaultAsync(t => t.Value == refreshTokenValue && t.UserId == userId);

        if (token is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (token.ExpiryTime <= _dateTime.Now)
            throw new UnauthorizedAccessException("Expired refresh token");

        return token;
    }

    private async Task ReplaceRefreshTokenAsync(
        RefreshToken current,
        string newTokenValue,
        int userId,
        CancellationToken cancellationToken)
    {
        _dbContext.RefreshToken.Remove(current);
        await RefreshTokenHelper.CreateAndSaveAsync(
            newTokenValue, 
            userId, 
            _config, 
            _dateTime, 
            _httpContextAccessor, 
            _dbContext, 
            _unitOfWork, 
            cancellationToken);
    }

    private LoginResponse BuildLoginResponse(ApplicationUser user, string accessToken, string refreshToken)
    {
        var userInfo = _mapper.Map<LoginUserInfo>(user);
        return new LoginResponse ()
        { 
            User = userInfo, 
            AccessToken = accessToken, 
            RefreshToken = refreshToken 
        };
    }

    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(v => v.AccessToken)
                .NotEmpty();

            RuleFor(v => v.RefreshToken)
                .MaximumLength(200)
                .NotEmpty();
        }
    }
}
