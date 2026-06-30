using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using Domain.Interfaces;
using DTO.Enums.Resume;
using DTO.Resumes;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringGenerateCommand(
    int JobId,
    int? ResumeId,
    ResumeTailoringGenerateSections Sections,
    IReadOnlyList<string> Keywords,
    TailoringWorkMode WorkMode) : ICommand<ResumeTailoringGenerateResponse>;

public sealed class ResumeTailoringGenerateCommandHandler : ICommandHandler<ResumeTailoringGenerateCommand, ResumeTailoringGenerateResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICreditService _creditService;
    private readonly IResumeTailoringAiService _aiService;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeTailoringGenerateCommandHandler(
        ICurrentUserService currentUserService,
        ICreditService creditService,
        IResumeTailoringAiService aiService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _creditService = creditService;
        _aiService = aiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResumeTailoringGenerateResponse> Handle(ResumeTailoringGenerateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var creditsRemaining = await _creditService.GetTailoringCreditsRemainingAsync(userId, cancellationToken);
        if (creditsRemaining == 0)
            throw new InsufficientCreditsException();

        var sections = ResolveSections(command.Sections);
        var aiOptions = new ResumeTailoringAiOptions
        {
            UserId = userId,
            JobId = command.JobId,
            SectionsToEnhance = sections,
            KeywordsToInject = command.Keywords
        };

        var result = await _aiService.TailorAsync(aiOptions, cancellationToken);

        await _creditService.DeductTailoringCreditAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCreditsRemaining = creditsRemaining == -1 ? -1 : creditsRemaining - 1;

        return new ResumeTailoringGenerateResponse
        {
            PreviousScore = result.ScoreBefore,
            NewScore = result.ScoreAfter,
            Changes = result.Changes,
            CreditsRemaining = newCreditsRemaining
        };
    }

    private static List<string> ResolveSections(ResumeTailoringGenerateSections sections)
    {
        var list = new List<string>();
        if (sections.Summary) list.Add("Summary");
        if (sections.Skills) list.Add("Skills");
        if (sections.WorkExperience) list.Add("WorkExperience");
        return list;
    }
}
