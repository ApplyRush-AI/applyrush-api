using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Admin;
using DTO.Enums.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries;

public sealed record AdminSubscriptionListGetQuery(SubscriptionPlan? Plan = null) : IQuery<IReadOnlyList<AdminSubscriptionListItemResponse>>;

public sealed class AdminSubscriptionListGetQueryHandler : IQueryHandler<AdminSubscriptionListGetQuery, IReadOnlyList<AdminSubscriptionListItemResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AdminSubscriptionListGetQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AdminSubscriptionListItemResponse>> Handle(AdminSubscriptionListGetQuery query, CancellationToken cancellationToken)
    {
        var baseQuery = _dbContext.UserSubscription
            .AsNoTracking()
            .Include(s => s.User)
            .AsQueryable();

        if (query.Plan.HasValue)
            baseQuery = baseQuery.Where(s => s.Plan == query.Plan.Value);

        var subscriptions = await baseQuery
            .OrderByDescending(s => s.Created)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<AdminSubscriptionListItemResponse>>(subscriptions);
    }
}
