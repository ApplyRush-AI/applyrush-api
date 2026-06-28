using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DTO.Admin;
using DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries;

public sealed record AdminUserListGetQuery(string? Search = null, int Page = 1, int PageSize = 20)
    : IQuery<PaginatedList<AdminUserListItemResponse>>;

public sealed class AdminUserListGetQueryHandler : IQueryHandler<AdminUserListGetQuery, PaginatedList<AdminUserListItemResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AdminUserListGetQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AdminUserListItemResponse>> Handle(AdminUserListGetQuery query, CancellationToken cancellationToken)
    {
        var baseQuery = _dbContext.UserSubscription
            .AsNoTracking()
            .Include(s => s.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            baseQuery = baseQuery.Where(s =>
                s.User.Email.ToLower().Contains(search) ||
                (s.User.FirstName != null && s.User.FirstName.ToLower().Contains(search)) ||
                (s.User.LastName != null && s.User.LastName.ToLower().Contains(search)));
        }

        var projected = baseQuery
            .OrderByDescending(s => s.User.Created)
            .ProjectTo<AdminUserListItemResponse>(_mapper.ConfigurationProvider);

        return await PaginatedList<AdminUserListItemResponse>.CreateAsync(projected, query.Page, query.PageSize, cancellationToken);
    }
}
