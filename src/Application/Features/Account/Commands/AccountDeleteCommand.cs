using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Users.Validators;
using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public sealed record AccountDeleteCommand(string Password) : ICommand;

public sealed class AccountDeleteCommandHandler : ICommandHandler<AccountDeleteCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public AccountDeleteCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }

    public async Task Handle(AccountDeleteCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _dbContext.User
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.Delete();
        RevokeAllSessions(user);

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void RevokeAllSessions(ApplicationUser user)
    {
        _dbContext.RefreshToken.RemoveRange(user.RefreshTokens);
    }
}

public sealed class AccountDeleteCommandValidator : AbstractValidator<AccountDeleteCommand>
{
    public AccountDeleteCommandValidator(
        ICurrentUserService currentUserService,
        CurrentPasswordValidator currentPasswordValidator)
    {
        RuleFor(cmd => cmd.Password)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(cmd => new CurrentPasswordValidatorData(cmd.Password, (int)currentUserService.UserId!))
                    .SetValidator(currentPasswordValidator)
                    .OverridePropertyName(nameof(AccountDeleteCommand.Password));
            });
    }
}
