using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Auth.ExternalAuth;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Authentication.Core;
using Application.Features.Authentication.Data;
using Application.Identity;
using AutoMapper;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Authentication;
using DTO.Enums.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Application.Features.Authentication.Commands.GoogleAuth;

public sealed record GoogleAuthCommand(string IdToken) : ICommand<LoginResponse>;

public sealed class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, LoginResponse>
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityContextAccessor _identityContextAccessor;
    private readonly IMapper _mapper;
    private readonly IdentityConfig _config;
    private readonly ILocalizationService _localizationService;

    public GoogleAuthCommandHandler(
        IExternalAuthService externalAuthService,
        UserManager<ApplicationUser> userManager,
        IApplicationUserManager applicationUserManager,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IIdentityContextAccessor identityContextAccessor,
        IOptions<IdentityConfig> config,
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _externalAuthService = externalAuthService;
        _userManager = userManager;
        _applicationUserManager = applicationUserManager;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _identityContextAccessor = identityContextAccessor;
        _config = config.Value;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    public async Task<LoginResponse> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        var authInfo = await _externalAuthService.AuthenticateAsync(request.IdToken, cancellationToken);

        var user = await _userManager.FindByEmailAsync(authInfo.Email);

        if (user is null)
            user = await RegisterNewUserAsync(authInfo, cancellationToken);

        else
            ValidateExistingUserStatus(user);

        var (token, _, newRefreshToken) = await _jwtTokenService.CreateAsync(user);
        await RefreshTokenHelper.CreateAndSaveAsync(newRefreshToken, user.Id, _config, _dateTime, _httpContextAccessor, _dbContext, _unitOfWork, cancellationToken);

        return BuildLoginResponse(user, token, newRefreshToken);
    }

    private async Task<ApplicationUser> RegisterNewUserAsync(ExternalAuthUserInfo authInfo, CancellationToken cancellationToken)
    {
        var insertData = new GoogleUserInsertData(authInfo.FirstName, authInfo.LastName, authInfo.Email);
        var user = ApplicationUser.Create(insertData, _dateTime, true);
        user.Activate();

        var randomPassword = _applicationUserManager.GenerateRandomPassword();
        await _applicationUserManager.CreateAsync(user, randomPassword);
        await _userManager.AddClaimAsync(user, new Claim("scope", "default"));
        await _userManager.AddToRoleAsync(user, UserRole.Customer);

        _identityContextAccessor.IdentityContext = new IdentityContextForApplicationCustom(new UserInfoById(user.Id));
        await UserFoundationDataHelper.CreateAndSaveAsync(user.Id, _dbContext, _unitOfWork, cancellationToken);

        return user;
    }

    private void ValidateExistingUserStatus(ApplicationUser user)
    {
        if (user.Status == UserStatus.Suspended)
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.suspended.error.message"));

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.inactive.error.message"));
    }

    private LoginResponse BuildLoginResponse(ApplicationUser user, string accessToken, string refreshToken)
    {
        var userInfo = _mapper.Map<LoginUserInfo>(user);

        return new LoginResponse()
        {
            User = userInfo,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}

public sealed class GoogleAuthCommandValidator : AbstractValidator<GoogleAuthCommand>
{
    public GoogleAuthCommandValidator()
    {
        RuleFor(c => c.IdToken)
            .NotEmpty();
    }
}
