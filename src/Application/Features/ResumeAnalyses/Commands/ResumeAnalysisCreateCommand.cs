using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Features.ResumeAnalyses.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeAnalyses;
using Domain.Interfaces;
using DTO.Response;
using DTO.Resumes;

namespace Application.Features.ResumeAnalyses.Commands;

public sealed record ResumeAnalysisCreateCommand : ICommand<ResumeAnalysisCreateResponse>;

public sealed class ResumeAnalysisCreateCommandHandler : ICommandHandler<ResumeAnalysisCreateCommand, ResumeAnalysisCreateResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreditService _creditService;
    private readonly IMapper _mapper;

    public ResumeAnalysisCreateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICreditService creditService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _creditService = creditService;
        _mapper = mapper;
    }

    public async Task<ResumeAnalysisCreateResponse> Handle(ResumeAnalysisCreateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var creditsRemaining = await _creditService.GetAnalysisCreditsRemainingAsync(userId, cancellationToken);
        if (creditsRemaining == 0)
            throw new InsufficientCreditsException();

        await _creditService.DeductAnalysisCreditAsync(userId, cancellationToken);

        var creditsUsed = creditsRemaining == -1 ? 0 : 1;

        var analysis = ResumeAnalysis.Create(new ResumeAnalysisInsertData(userId, creditsUsed));

        _dbContext.ResumeAnalysis.Add(analysis);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCreditsRemaining = creditsRemaining == -1 ? -1 : creditsRemaining - 1;

        return new ResumeAnalysisCreateResponse
        {
            Id = analysis.Id,
            Status = _mapper.Map<ListItemBaseResponse>(analysis.Status),
            CreditsRemaining = newCreditsRemaining
        };
    }
}
