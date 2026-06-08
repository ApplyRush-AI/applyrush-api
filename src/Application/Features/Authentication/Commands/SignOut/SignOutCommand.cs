using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Commands.SignOut;

public sealed record SignOutCommand(string RefreshToken) : ICommand;

public sealed class SignOutCommandHandler : ICommandHandler<SignOutCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public SignOutCommandHandler(
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

    public async Task Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var token = await _dbContext.RefreshToken
            .FirstOrDefaultAsync(t => t.Value == request.RefreshToken && t.UserId == userId, cancellationToken);

        if (token is null)
            throw new UnauthorizedAccessException(_localizationService.GetValue("refreshToken.notFound.error.message"));

        _dbContext.RefreshToken.Remove(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class SignOutCommandValidator : AbstractValidator<SignOutCommand>
{
    public SignOutCommandValidator()
    {
        RuleFor(c => c.RefreshToken)
            .NotEmpty();
    }
}
