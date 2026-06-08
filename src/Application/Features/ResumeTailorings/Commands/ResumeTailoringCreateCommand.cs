using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Features.ResumeTailorings.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using Domain.Interfaces;
using DTO.Resumes;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringCreateCommand(
    int JobId,
    int? ResumeId,
    IReadOnlyList<string> SectionsToEnhance,
    IReadOnlyList<string> KeywordsToInject) : ICommand<ResumeTailoringCreateResponse>;

public sealed class ResumeTailoringCreateCommandHandler : ICommandHandler<ResumeTailoringCreateCommand, ResumeTailoringCreateResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreditService _creditService;
    private readonly IMapper _mapper;

    public ResumeTailoringCreateCommandHandler(
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

    public async Task<ResumeTailoringCreateResponse> Handle(ResumeTailoringCreateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var creditsRemaining = await _creditService.GetTailoringCreditsRemainingAsync(userId, cancellationToken);
        if (creditsRemaining == 0)
            throw new InsufficientCreditsException();

        await _creditService.DeductTailoringCreditAsync(userId, cancellationToken);

        var creditsUsed = creditsRemaining == -1 ? 0 : 1;

        var tailoring = ResumeTailoring.Create(new ResumeTailoringInsertData(
            userId,
            command.ResumeId,
            command.JobId,
            command.SectionsToEnhance,
            command.KeywordsToInject,
            creditsUsed));

        _dbContext.ResumeTailoring.Add(tailoring);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCreditsRemaining = creditsRemaining == -1 ? -1 : creditsRemaining - 1;

        return new ResumeTailoringCreateResponse
        {
            TailoringId = tailoring.Id,
            ScoreBefore = 0,
            ScoreAfter = 0,
            Summary = null,
            Experience = [],
            HighlightedSkills = [],
            MissingSkills = [],
            CreditsRemaining = newCreditsRemaining,
            Status = _mapper.Map<DTO.Response.ListItemBaseResponse>(tailoring.Status)
        };
    }
}
