using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.MessageBroker;
using Domain.Entities.Jobs.JobListings;
using DTO.Enums;
using DTO.Enums.Job;
using DTO.MessageBroker.Messages.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.JobOffers.Commands;

public sealed record JobSyncCommand(int PagesPerQuery = 3) : ICommand;

public sealed class JobSyncCommandHandler : ICommandHandler<JobSyncCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobProvider _jobProvider;
    private readonly IJobSyncState _syncState;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<JobSyncCommandHandler> _logger;

    public JobSyncCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IJobProvider jobProvider,
        IJobSyncState syncState,
        IMessagePublisher messagePublisher,
        ILogger<JobSyncCommandHandler> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _jobProvider = jobProvider;
        _syncState = syncState;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(JobSyncCommand command, CancellationToken cancellationToken)
    {
        var currentState = await _syncState.GetAsync(cancellationToken);
        if (currentState.Status == JobSyncStatus.Running)
        {
            _logger.LogWarning("[JobSync] Sync is already running. Skipping.");
            return;
        }

        await _syncState.SetAsync(new JobSyncStateData { Status = JobSyncStatus.Running }, cancellationToken);

        var jobsIndexed = 0;
        var failedPages = 0;
        string? lastError = null;

        try
        {
            var jobFunctions = await _dbContext.JobFunction
                .Where(f => f.Status == Status.Active && !f.Children.Any())
                .Select(f => new { f.Id, f.Name })
                .ToListAsync(cancellationToken);

            var queries = jobFunctions.Select(f => f.Name).ToList();

            _logger.LogInformation("[JobSync] Starting sync for {Count} job functions.", queries.Count);

            foreach (var query in queries)
            {
                for (var page = 1; page <= command.PagesPerQuery; page++)
                {
                    try
                    {
                        var pageIndexed = await SyncPageAsync(query, page, jobFunctions.Select(f => (f.Id, f.Name)).ToList(), cancellationToken);
                        if (pageIndexed == 0)
                            break;
                        jobsIndexed += pageIndexed;
                    }
                    catch (Exception ex)
                    {
                        failedPages++;
                        lastError = $"{query} p{page}: {ex.Message}";
                        _logger.LogError(ex, "[JobSync] Page failed (continuing). Query='{Query}' Page={Page}.", query, page);
                    }
                }
            }

            var allFailed = jobsIndexed == 0 && failedPages > 0;
            await _syncState.SetAsync(new JobSyncStateData
            {
                Status = allFailed ? JobSyncStatus.Failed : JobSyncStatus.Idle,
                LastSyncedAt = DateTime.UtcNow,
                JobsIndexed = jobsIndexed,
                FailedPages = failedPages,
                ErrorMessage = failedPages > 0 ? lastError : null
            }, cancellationToken);

            _logger.LogInformation("[JobSync] Completed. Jobs synced: {Count}. Failed pages: {Failed}.", jobsIndexed, failedPages);

            if (!allFailed && jobsIndexed > 0)
                await _messagePublisher.PublishAsync(new JobDigestTriggerMessage(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[JobSync] Sync aborted.");
            await _syncState.SetAsync(new JobSyncStateData
            {
                Status = JobSyncStatus.Failed,
                LastSyncedAt = DateTime.UtcNow,
                JobsIndexed = jobsIndexed,
                FailedPages = failedPages,
                ErrorMessage = ex.Message
            }, cancellationToken);
        }
    }

    private async Task<int> SyncPageAsync(string query, int page, List<(int Id, string Name)> jobFunctions, CancellationToken cancellationToken)
    {
        var jobs = await _jobProvider.FetchJobsAsync(query, page, cancellationToken);
        if (jobs.Count == 0)
            return 0;

        var externalIds = jobs.Select(j => j.ExternalId).ToList();
        var existingJobs = await _dbContext.JobListing
            .Include(j => j.JobFunctions)
            .Where(j => externalIds.Contains(j.ExternalId))
            .ToListAsync(cancellationToken);
        var existingMap = existingJobs.ToDictionary(j => j.ExternalId);

        var trackedForRollback = new List<JobListing>();

        foreach (var jobData in jobs)
        {
            JobListing listing;
            if (existingMap.TryGetValue(jobData.ExternalId, out var existing))
            {
                existing.Update(jobData);
                listing = existing;
            }
            else
            {
                listing = JobListing.Create(jobData);
                _dbContext.JobListing.Add(listing);
            }

            listing.SetJobFunctions(MatchJobFunctions(listing.Title, jobFunctions));
            trackedForRollback.Add(listing);
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[JobSync] Synced page {Page} for '{Query}': {Count} jobs.", page, query, jobs.Count);
            return jobs.Count;
        }
        catch
        {
            // Detach so a save failure on this page doesn't poison the next page's SaveChanges.
            foreach (var entity in trackedForRollback)
                _dbContext.Entry(entity).State = EntityState.Detached;
            throw;
        }
    }

    // Returns IDs of all job functions whose name is a substring of the title.
    private static IEnumerable<int> MatchJobFunctions(string title, List<(int Id, string Name)> jobFunctions)
    {
        var titleLower = title.ToLowerInvariant();
        return jobFunctions
            .Where(f => titleLower.Contains(f.Name.ToLowerInvariant()))
            .Select(f => f.Id);
    }
}
