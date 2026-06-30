using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Queries;

public sealed record InvoiceListGetQuery : IQuery<IReadOnlyList<InvoiceResponse>>;

public sealed class InvoiceListGetQueryHandler : IQueryHandler<InvoiceListGetQuery, IReadOnlyList<InvoiceResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;

    public InvoiceListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IStripeService stripeService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _stripeService = stripeService;
    }

    public async Task<IReadOnlyList<InvoiceResponse>> Handle(InvoiceListGetQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.UserSubscription
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _currentUserService.UserId, cancellationToken);

        if (subscription == null)
            return [];

        return await _stripeService.GetInvoicesAsync(subscription.StripeCustomerId, cancellationToken);
    }
}
