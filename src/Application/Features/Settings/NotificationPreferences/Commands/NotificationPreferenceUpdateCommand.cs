using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Settings;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Settings.NotificationPreferences.Commands;

public sealed record NotificationPreferenceUpdateCommand(
    bool? NewJobMatches,
    bool? ApplicationUpdates,
    bool? WeeklyDigest,
    bool? ProfileViews,
    bool? MarketingEmails
    ) : ICommand, IUserNotificationPreferenceUpdateData;

public sealed class NotificationPreferenceUpdateCommandHandler : ICommandHandler<NotificationPreferenceUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationPreferenceUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(NotificationPreferenceUpdateCommand command, CancellationToken cancellationToken)
    {
        var pref = await _dbContext.UserNotificationPreference
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserNotificationPreference>();

        pref.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class NotificationPreferenceUpdateCommandValidator : AbstractValidator<NotificationPreferenceUpdateCommand>
{
    public NotificationPreferenceUpdateCommandValidator() { }
}
