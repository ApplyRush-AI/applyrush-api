using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Interfaces;
using DTO.Profile.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sessions.Queries;

public sealed record SessionListGetQuery : IQuery<IReadOnlyList<SessionResponse>>;

public sealed class SessionListGetQueryHandler : IQueryHandler<SessionListGetQuery, IReadOnlyList<SessionResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IDateTime _dateTime;

    public SessionListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper,
        IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<IReadOnlyList<SessionResponse>> Handle(SessionListGetQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.RefreshToken
            .Where(t => t.UserId == _currentUserService.UserId && t.ExpiryTime > _dateTime.Now)
            .OrderByDescending(t => t.LastUsedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<SessionResponse>>(sessions);
    }
}
