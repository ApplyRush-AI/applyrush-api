using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Authentication.Core;
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

namespace Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(string Username, string Password): ICommand<LoginResponse>
{
    public static LoginCommand Of(string username, string password)
        => new(username, password);
}

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityConfig _config;
    private readonly ILocalizationService _localizationService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime,
        IOptions<IdentityConfig> config,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService,
        IMapper mapper)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _config = config.Value;
        _localizationService = localizationService;
        _mapper = mapper;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            ValidateUserStatus(user);

            var (token, _, newRefreshToken) = await _jwtTokenService.CreateAsync(user);
            await RefreshTokenHelper.CreateAndSaveAsync(newRefreshToken, user.Id, _config, _dateTime, _httpContextAccessor, _dbContext, _unitOfWork, cancellationToken);

            await _userManager.UpdateAsync(user);
            return BuildLoginResponse(user, token, newRefreshToken);
        }

        throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.wrongCredentials.error.message"));
    }

    private async Task ValidateUserStatus(ApplicationUser user)
    {
        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.unverified.error.message"));

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.locked.error.message"));

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
            RefreshToken = refreshToken
        };
    }

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(lc => lc.Username)
                .NotEmpty()
                .EmailAddress();

            RuleFor(lc => lc.Password)
                .NotEmpty();
        }
    }
}
