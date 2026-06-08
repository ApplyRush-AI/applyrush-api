using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeAnalyses.Commands;

public sealed record ResumeAnalysisFixAllCommand(int Id) : ICommand<IReadOnlyList<ResumeAnalysisFixResponse>>;

public sealed class ResumeAnalysisFixAllCommandHandler : ICommandHandler<ResumeAnalysisFixAllCommand, IReadOnlyList<ResumeAnalysisFixResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IResumeAnalysisAiService _aiService;
    private readonly IMapper _mapper;

    public ResumeAnalysisFixAllCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IResumeAnalysisAiService aiService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _aiService = aiService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeAnalysisFixResponse>> Handle(ResumeAnalysisFixAllCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == command.Id && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeAnalysis), command.Id);

        var options = _mapper.Map<ResumeAnalysisFixAllAiOptions>(analysis);
        var results = await _aiService.FixAllIssuesAsync(options, cancellationToken);

        return _mapper.Map<IReadOnlyList<ResumeAnalysisFixResponse>>(results);
    }
}
