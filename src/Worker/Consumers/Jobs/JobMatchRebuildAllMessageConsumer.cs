using Application.Common.Interfaces;
using Application.Features.JobOffers.Commands;
using DTO.MessageBroker.Messages.Jobs;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Worker.Consumers.Jobs;

public sealed class JobMatchRebuildAllMessageConsumer : IConsumer<JobMatchRebuildAllMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISender _mediator;

    public JobMatchRebuildAllMessageConsumer(IApplicationDbContext dbContext, ISender mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<JobMatchRebuildAllMessage> context)
    {
        var jobIds = await _dbContext.JobListing
            .AsNoTracking()
            .Select(j => j.Id)
            .ToListAsync(context.CancellationToken);

        foreach (var jobId in jobIds)
            await _mediator.Send(new JobMatchAllUsersCommand(jobId), context.CancellationToken);
    }
}
