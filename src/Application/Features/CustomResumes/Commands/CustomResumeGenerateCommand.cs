using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.CustomResume;
using Domain.Interfaces;
using DTO.Enums.Resume;
using DTO.Resumes;

namespace Application.Features.CustomResumes.Commands;

public sealed record CustomResumeGenerateCommand(
    int JobId,
    int? ResumeId,
    ResumeTailoringGenerateSections Sections,
    IReadOnlyList<string> Keywords,
    TailoringWorkMode WorkMode) : ICommand<CustomResumeResultResponse>;

public sealed class CustomResumeGenerateCommandHandler : ICommandHandler<CustomResumeGenerateCommand, CustomResumeResultResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICreditService _creditService;
    private readonly ICustomResumeAiService _aiService;
    private readonly IUnitOfWork _unitOfWork;

    public CustomResumeGenerateCommandHandler(
        ICurrentUserService currentUserService,
        ICreditService creditService,
        ICustomResumeAiService aiService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _creditService = creditService;
        _aiService = aiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomResumeResultResponse> Handle(CustomResumeGenerateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var creditsRemaining = await _creditService.GetTailoringCreditsRemainingAsync(userId, cancellationToken);
        if (creditsRemaining == 0)
            throw new InsufficientCreditsException();

        var result = await _aiService.GenerateAsync(new CustomResumeGenerateOptions
        {
            UserId = userId,
            JobId = command.JobId,
            SectionsToEnhance = ResolveSections(command.Sections),
            KeywordsToInject = command.Keywords,
            WorkMode = command.WorkMode
        }, cancellationToken);

        await _creditService.DeductTailoringCreditAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCreditsRemaining = creditsRemaining == -1 ? -1 : creditsRemaining - 1;

        return new CustomResumeResultResponse
        {
            Content = result.Content,
            ScoreBefore = result.ScoreBefore,
            ScoreAfter = result.ScoreAfter,
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
