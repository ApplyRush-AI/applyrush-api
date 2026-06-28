using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Localization.Extensions;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Interfaces;
using DTO.Enums.Subscription;
using DTO.Subscription;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Commands;

public sealed record SubscriptionCheckoutCreateCommand(
    SubscriptionPlan Plan,
    BillingInterval Interval
) : ICommand<CheckoutUrlResponse>;

public sealed class SubscriptionCheckoutCreateCommandHandler : ICommandHandler<SubscriptionCheckoutCreateCommand, CheckoutUrlResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionCheckoutCreateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CheckoutUrlResponse> Handle(SubscriptionCheckoutCreateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw NotFoundException.New<UserSubscription>();

        var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(
            subscription.StripeCustomerId,
            command.Plan,
            command.Interval,
            cancellationToken);

        return new CheckoutUrlResponse(checkoutUrl);
    }
}

public sealed class SubscriptionCheckoutCreateCommandValidator : AbstractValidator<SubscriptionCheckoutCreateCommand>
{
    public SubscriptionCheckoutCreateCommandValidator()
    {
        RuleFor(c => c.Plan)
            .IsInEnum()
            .Must(p => p != SubscriptionPlan.Free)
            .WithLocalizationKey("subscription.checkout.freePlanNotAllowed");

        RuleFor(c => c.Interval)
            .IsInEnum();
    }
}
