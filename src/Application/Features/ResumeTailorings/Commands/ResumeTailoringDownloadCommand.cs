using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Tailoring.ResumeTailorings;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringDownloadCommand(int Id) : ICommand<byte[]>;

public sealed class ResumeTailoringDownloadCommandHandler : ICommandHandler<ResumeTailoringDownloadCommand, byte[]>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPdfExportService _pdfExportService;

    public ResumeTailoringDownloadCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IPdfExportService pdfExportService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _pdfExportService = pdfExportService;
    }

    public async Task<byte[]> Handle(ResumeTailoringDownloadCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var exists = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .AnyAsync(t => t.Id == command.Id && t.UserId == userId, cancellationToken);

        if (!exists)
            throw new NotFoundException(nameof(ResumeTailoring), command.Id);

        var stream = await _pdfExportService.ExportTailoringAsPdfAsync(command.Id, cancellationToken);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
