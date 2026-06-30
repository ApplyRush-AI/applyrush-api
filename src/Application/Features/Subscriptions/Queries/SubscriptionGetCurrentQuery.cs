using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums.Subscription;
using DTO.Response;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Queries;

public sealed record SubscriptionGetCurrentQuery : IQuery<SubscriptionResponse>;

public sealed class SubscriptionGetCurrentQueryHandler : IQueryHandler<SubscriptionGetCurrentQuery, SubscriptionResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public SubscriptionGetCurrentQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SubscriptionResponse> Handle(SubscriptionGetCurrentQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.UserSubscription
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _currentUserService.UserId, cancellationToken);

        // A user with no subscription record is effectively on the Free plan — never 404 here.
        return subscription == null
            ? FreeSubscription()
            : _mapper.Map<SubscriptionResponse>(subscription);
    }

    private SubscriptionResponse FreeSubscription() => new()
    {
        Plan = _mapper.Map<ListItemBaseResponse>(SubscriptionPlan.Free),
        Status = _mapper.Map<ListItemBaseResponse>(SubscriptionStatus.Active),
        Price = SubscriptionPrices.Free
    };
}
