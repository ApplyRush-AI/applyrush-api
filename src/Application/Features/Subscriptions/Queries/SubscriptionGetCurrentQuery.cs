using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Subscriptions.UserSubscriptions;
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
            .FirstOrDefaultAsync(s => s.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserSubscription>();

        return _mapper.Map<SubscriptionResponse>(subscription);
    }
}
