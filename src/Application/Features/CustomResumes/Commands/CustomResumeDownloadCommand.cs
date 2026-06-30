using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using DTO.Resumes;

namespace Application.Features.CustomResumes.Commands;

public sealed record CustomResumeDownloadCommand(
    TailoredResumeContent Content,
    TailoredResumeStyle? Style = null) : ICommand<byte[]>;

public sealed class CustomResumeDownloadCommandHandler : ICommandHandler<CustomResumeDownloadCommand, byte[]>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomResumePdfService _pdfService;

    public CustomResumeDownloadCommandHandler(
        ICurrentUserService currentUserService,
        ICustomResumePdfService pdfService)
    {
        _currentUserService = currentUserService;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(CustomResumeDownloadCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var stream = await _pdfService.RenderAsync(command.Content, command.Style, userId, cancellationToken);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
