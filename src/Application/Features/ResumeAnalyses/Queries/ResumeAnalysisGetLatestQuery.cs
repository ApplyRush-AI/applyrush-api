using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeAnalyses.Queries;

public sealed record ResumeAnalysisGetLatestQuery : IQuery<ResumeAnalysisResponse>;

public sealed class ResumeAnalysisGetLatestQueryHandler : IQueryHandler<ResumeAnalysisGetLatestQuery, ResumeAnalysisResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeAnalysisGetLatestQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ResumeAnalysisResponse> Handle(ResumeAnalysisGetLatestQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Created)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeAnalysis));

        return _mapper.Map<ResumeAnalysisResponse>(analysis);
    }
}
