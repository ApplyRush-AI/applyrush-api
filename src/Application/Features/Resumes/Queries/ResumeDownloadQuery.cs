using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Resumes.Resume;
using Domain.Interfaces;
using DTO.Enums;
using DTO.Medias;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Queries;

public sealed record ResumeDownloadQuery(int ResumeId) : IQuery<FileModel>;

public sealed class ResumeDownloadQueryHandler : IQueryHandler<ResumeDownloadQuery, FileModel>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMediaStorage _mediaStorage;

    public ResumeDownloadQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMediaStorage mediaStorage)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mediaStorage = mediaStorage;
    }

    public async Task<FileModel> Handle(ResumeDownloadQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var resume = await _dbContext.Resume
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.ResumeId && r.UserId == userId && r.Status == Status.Active, cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        var mediaItem = resume.Media.Items.FirstOrDefault()
            ?? throw NotFoundException.New<Resume>();

        var (item, stream) = await resume.Media.GetContent(mediaItem.Id, resume.Id, _mediaStorage);

        var contentType = resume.FileType switch
        {
            DTO.Enums.Resume.ResumeFileType.Pdf  => "application/pdf",
            DTO.Enums.Resume.ResumeFileType.Word => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _                                    => "application/octet-stream"
        };

        return new FileModel(item.Name, stream, contentType);
    }
}
