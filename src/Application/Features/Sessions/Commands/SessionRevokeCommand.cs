using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.RefreshTokens;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sessions.Commands;

public sealed record SessionRevokeCommand(string Token) : ICommand;

public sealed class SessionRevokeCommandHandler : ICommandHandler<SessionRevokeCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public SessionRevokeCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SessionRevokeCommand command, CancellationToken cancellationToken)
    {
        var token = await _dbContext.RefreshToken
            .FirstOrDefaultAsync(t => t.Value == command.Token && t.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<RefreshToken>();

        _dbContext.RefreshToken.Remove(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class SessionRevokeCommandValidator : AbstractValidator<SessionRevokeCommand>
{
    public SessionRevokeCommandValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
    }
}
