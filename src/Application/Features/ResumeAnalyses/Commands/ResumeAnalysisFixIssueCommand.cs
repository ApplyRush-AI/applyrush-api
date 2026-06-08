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

public sealed record ResumeAnalysisFixIssueCommand(int Id, string IssueId) : ICommand<ResumeAnalysisFixResponse>;

public sealed class ResumeAnalysisFixIssueCommandHandler : ICommandHandler<ResumeAnalysisFixIssueCommand, ResumeAnalysisFixResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IResumeAnalysisAiService _aiService;
    private readonly IMapper _mapper;

    public ResumeAnalysisFixIssueCommandHandler(
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

    public async Task<ResumeAnalysisFixResponse> Handle(ResumeAnalysisFixIssueCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == command.Id && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeAnalysis), command.Id);

        var options = _mapper.Map<ResumeAnalysisFixAiOptions>(analysis) with { IssueId = command.IssueId };
        var result = await _aiService.FixIssueAsync(options, cancellationToken);

        return _mapper.Map<ResumeAnalysisFixResponse>(result);
    }
}
