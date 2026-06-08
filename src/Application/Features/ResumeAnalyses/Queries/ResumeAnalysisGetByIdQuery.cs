using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeAnalyses.Queries;

public sealed record ResumeAnalysisGetByIdQuery(int Id) : IQuery<ResumeAnalysisResponse>;

public sealed class ResumeAnalysisGetByIdQueryHandler : IQueryHandler<ResumeAnalysisGetByIdQuery, ResumeAnalysisResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeAnalysisGetByIdQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ResumeAnalysisResponse> Handle(ResumeAnalysisGetByIdQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.Id && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeAnalysis), query.Id);

        return _mapper.Map<ResumeAnalysisResponse>(analysis);
    }
}
