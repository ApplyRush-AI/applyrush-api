using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using DTO.MessageBroker.Messages.Jobs;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Worker.Consumers.Jobs;

public sealed class JobMatchRebuildUserMessageConsumer : IConsumer<JobMatchRebuildUserMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMatchScoringService _matchScoringService;

    public JobMatchRebuildUserMessageConsumer(IApplicationDbContext dbContext, IMatchScoringService matchScoringService)
    {
        _dbContext = dbContext;
        _matchScoringService = matchScoringService;
    }

    public async Task Consume(ConsumeContext<JobMatchRebuildUserMessage> context)
    {
        var jobIds = await _dbContext.JobListing
            .AsNoTracking()
            .Select(j => j.Id)
            .ToListAsync(context.CancellationToken);

        foreach (var jobId in jobIds)
            await _matchScoringService.ComputeAndSaveAsync(context.Message.UserId, jobId, context.CancellationToken);
    }
}
