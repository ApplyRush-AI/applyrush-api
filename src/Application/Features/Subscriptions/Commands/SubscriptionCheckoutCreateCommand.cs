using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Localization.Extensions;
using Application.Features.Subscriptions.Data;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Entities.User;
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
        var userId = (int)_currentUserService.UserId!;
        var subscription = await GetOrCreateSubscriptionAsync(userId, cancellationToken);

        var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(
            subscription.StripeCustomerId,
            command.Plan,
            command.Interval,
            cancellationToken);

        return new CheckoutUrlResponse(checkoutUrl);
    }

    private async Task<UserSubscription> GetOrCreateSubscriptionAsync(int userId, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (subscription is not null)
            return subscription;

        var user = await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw NotFoundException.New<ApplicationUser>();

        var customerId = await _stripeService.GetOrCreateCustomerAsync(userId, user.Email!, cancellationToken);

        subscription = UserSubscription.Create(new UserSubscriptionInsertData(userId, customerId));
        await _dbContext.UserSubscription.AddAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subscription;
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
