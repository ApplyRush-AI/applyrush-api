using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums;
using DTO.Enums.JobApplication;
using DTO.JobApplications;
using DTO.Medias;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text.Json;

namespace Application.Features.Account.Queries;

public sealed record AccountDataExportQuery : IQuery<FileModel>;

public sealed class AccountDataExportQueryHandler : IQueryHandler<AccountDataExportQuery, FileModel>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AccountDataExportQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<FileModel> Handle(AccountDataExportQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var profile = await _dbContext.UserProfile
            .AsNoTracking()
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences).ThenInclude(w => w.Bullets)
            .Include(p => p.Educations)
            .Include(p => p.JobTypePreferences)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var applications = await _dbContext.JobApplication
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Status != ApplicationStatus.Deleted)
            .Include(a => a.Job)
            .OrderByDescending(a => a.Created)
            .ToListAsync(cancellationToken);

        var savedJobIds = await _dbContext.UserSavedJob
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == Status.Active)
            .Select(s => s.JobId)
            .ToListAsync(cancellationToken);

        var notificationPrefs = await _dbContext.UserNotificationPreference
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.UserId == userId, cancellationToken);

        var jobPrefs = await _dbContext.UserJobPreference
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.UserId == userId, cancellationToken);

        var mappedApplications = _mapper.Map<IReadOnlyList<JobApplicationResponse>>(applications);

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            await AddJsonEntry(archive, "profile.json", profile, cancellationToken);
            await AddJsonEntry(archive, "applications.json", mappedApplications, cancellationToken);
            await AddJsonEntry(archive, "saved_jobs.json", new { SavedJobIds = savedJobIds }, cancellationToken);
            await AddJsonEntry(archive, "settings.json", new { Notifications = notificationPrefs, JobPreferences = jobPrefs }, cancellationToken);
            await AddJsonEntry(archive, "credit_history.json", new { Credits = Array.Empty<object>() }, cancellationToken);
        }

        memoryStream.Position = 0;
        return new FileModel("account-data-export.zip", memoryStream, "application/zip");
    }

    private static async Task AddJsonEntry(ZipArchive archive, string entryName, object? data, CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        await using var entryStream = entry.Open();
        await JsonSerializer.SerializeAsync(entryStream, data, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
    }
}

