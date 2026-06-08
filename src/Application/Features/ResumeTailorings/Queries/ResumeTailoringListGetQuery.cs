using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Queries;

public sealed record ResumeTailoringListGetQuery : IQuery<IReadOnlyList<ResumeTailoringResponse>>;

public sealed class ResumeTailoringListGetQueryHandler : IQueryHandler<ResumeTailoringListGetQuery, IReadOnlyList<ResumeTailoringResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeTailoringListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeTailoringResponse>> Handle(ResumeTailoringListGetQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var tailorings = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Created)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ResumeTailoringResponse>>(tailorings);
    }
}
