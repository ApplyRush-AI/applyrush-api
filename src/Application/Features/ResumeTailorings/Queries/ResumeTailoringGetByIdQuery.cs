using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Queries;

public sealed record ResumeTailoringGetByIdQuery(int Id) : IQuery<ResumeTailoringResponse>;

public sealed class ResumeTailoringGetByIdQueryHandler : IQueryHandler<ResumeTailoringGetByIdQuery, ResumeTailoringResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeTailoringGetByIdQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ResumeTailoringResponse> Handle(ResumeTailoringGetByIdQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var tailoring = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.Id && t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeTailoring), query.Id);

        return _mapper.Map<ResumeTailoringResponse>(tailoring);
    }
}
