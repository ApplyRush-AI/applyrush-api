using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using DTO.Enums.Resume;
using DTO.Resumes;

namespace Application.Features.CustomResumes.Commands;

public sealed record CustomResumeRewriteCommand(
    TailoredResumeContent Content,
    AiRewriteAction Action,
    string? FreeFormInstruction) : ICommand<TailoredResumeContent>;

public sealed class CustomResumeRewriteCommandHandler : ICommandHandler<CustomResumeRewriteCommand, TailoredResumeContent>
{
    private readonly ICustomResumeAiService _aiService;

    public CustomResumeRewriteCommandHandler(ICustomResumeAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<TailoredResumeContent> Handle(CustomResumeRewriteCommand command, CancellationToken cancellationToken)
    {
        return await _aiService.RewriteAsync(command.Content, command.Action, command.FreeFormInstruction, cancellationToken);
    }
}
