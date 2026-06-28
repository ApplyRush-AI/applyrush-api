using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Localization;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Interfaces;
using DTO.Enums.Subscription;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Commands;

public sealed record SubscriptionCancelCommand : ICommand<SubscriptionCancelResponse>;

public sealed class SubscriptionCancelCommandHandler : ICommandHandler<SubscriptionCancelCommand, SubscriptionCancelResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTimeProvider;
    private readonly ILocalizationService _localizationService;

    public SubscriptionCancelCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IStripeService stripeService,
        IUnitOfWork unitOfWork,
        IDateTime dateTimeProvider,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _localizationService = localizationService;
    }

    public async Task<SubscriptionCancelResponse> Handle(SubscriptionCancelCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw NotFoundException.New<UserSubscription>();

        if (subscription.StripeSubscriptionId is null)
            throw new InvalidOperationException(_localizationService.GetValue("subscription.cancel.noActivePaidSubscription"));

        if (subscription.Status == SubscriptionStatus.Canceled)
            throw new InvalidOperationException(_localizationService.GetValue("subscription.cancel.alreadyCanceled"));

        await _stripeService.CancelSubscriptionAtPeriodEndAsync(subscription.StripeSubscriptionId, cancellationToken);

        var canceledAt = _dateTimeProvider.Now;
        subscription.Cancel(canceledAt);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubscriptionCancelResponse(canceledAt, subscription.CurrentPeriodEnd);
    }
}

