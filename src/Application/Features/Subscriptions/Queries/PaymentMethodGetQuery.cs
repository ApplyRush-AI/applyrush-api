using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Subscriptions.UserSubscriptions;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Queries;

public sealed record PaymentMethodGetQuery : IQuery<PaymentMethodResponse?>;

public sealed class PaymentMethodGetQueryHandler : IQueryHandler<PaymentMethodGetQuery, PaymentMethodResponse?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;

    public PaymentMethodGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IStripeService stripeService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _stripeService = stripeService;
    }

    public async Task<PaymentMethodResponse?> Handle(PaymentMethodGetQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.UserSubscription
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserSubscription>();

        return await _stripeService.GetPaymentMethodAsync(subscription.StripeCustomerId, cancellationToken);
    }
}
