using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Jobs.JobListings;
using DTO.Enums;
using DTO.Enums.Job;
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
    private readonly ILogger<JobSyncCommandHandler> _logger;

    public JobSyncCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IJobProvider jobProvider,
        IJobSyncState syncState,
        ILogger<JobSyncCommandHandler> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _jobProvider = jobProvider;
        _syncState = syncState;
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

        try
        {
            var queries = await _dbContext.JobFunction
                .Where(f => f.Status == Status.Active)
                .Select(f => f.Name)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("[JobSync] Starting sync for {Count} job functions.", queries.Count);

            foreach (var query in queries)
            {
                for (var page = 1; page <= command.PagesPerQuery; page++)
                {
                    IReadOnlyList<Models.JobListingData> jobs;

                    try
                    {
                        jobs = await _jobProvider.FetchJobsAsync(query, page, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[JobSync] Failed to fetch page {Page} for query '{Query}'.", page, query);
                        continue;
                    }

                    if (jobs.Count == 0)
                        break;

                    var externalIds = jobs.Select(j => j.ExternalId).ToList();
                    var existingJobs = await _dbContext.JobListing
                        .Where(j => externalIds.Contains(j.ExternalId))
                        .ToListAsync(cancellationToken);

                    var existingMap = existingJobs.ToDictionary(j => j.ExternalId);

                    foreach (var jobData in jobs)
                    {
                        if (existingMap.TryGetValue(jobData.ExternalId, out var existing))
                        {
                            existing.Update(jobData);
                        }
                        else
                        {
                            _dbContext.JobListing.Add(JobListing.Create(jobData));
                        }

                        jobsIndexed++;
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("[JobSync] Synced page {Page} for '{Query}': {Count} jobs.", page, query, jobs.Count);
                }
            }

            await _syncState.SetAsync(new JobSyncStateData
            {
                Status = JobSyncStatus.Idle,
                LastSyncedAt = DateTime.UtcNow,
                JobsIndexed = jobsIndexed
            }, cancellationToken);

            _logger.LogInformation("[JobSync] Completed. Total jobs synced: {Count}.", jobsIndexed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[JobSync] Sync failed.");
            await _syncState.SetAsync(new JobSyncStateData
            {
                Status = JobSyncStatus.Failed,
                LastSyncedAt = DateTime.UtcNow,
                JobsIndexed = jobsIndexed,
                ErrorMessage = ex.Message
            }, cancellationToken);
        }
    }
}
