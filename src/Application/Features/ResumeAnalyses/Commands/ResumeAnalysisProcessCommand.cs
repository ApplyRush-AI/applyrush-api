using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using Application.Features.ResumeAnalyses.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Enums.Resume;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeAnalyses.Commands;

public sealed record ResumeAnalysisProcessCommand(int AnalysisId) : ICommand;

public sealed class ResumeAnalysisProcessCommandHandler : ICommandHandler<ResumeAnalysisProcessCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeAnalysisAiService _aiService;
    private readonly IMapper _mapper;

    public ResumeAnalysisProcessCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IResumeAnalysisAiService aiService,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _mapper = mapper;
    }

    public async Task Handle(ResumeAnalysisProcessCommand command, CancellationToken cancellationToken)
    {
        var analysis = await _dbContext.ResumeAnalysis
            .FirstOrDefaultAsync(a => a.Id == command.AnalysisId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeAnalysis), command.AnalysisId);

        try
        {
            var options = _mapper.Map<ResumeAnalysisAiOptions>(analysis);
            var result = await _aiService.AnalyzeAsync(options, cancellationToken);

            var completeData = _mapper.Map<ResumeAnalysisCompleteData>(result);
            analysis.Complete(completeData with { Status = ResumeAnalysisStatus.Completed });
        }
        catch
        {
            analysis.MarkFailed();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
