using Application.Features.JobOffers.Commands;
using DTO.MessageBroker.Messages.Jobs;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Jobs;

public sealed class JobDigestTriggerMessageConsumer : IConsumer<JobDigestTriggerMessage>
{
    // Match scores are computed asynchronously (one JobMatchAllUsersMessage per new job), so we wait
    // for that backlog to drain before building the digest, otherwise users would miss freshly synced jobs.
    private static readonly TimeSpan ScoringDrainDelay = TimeSpan.FromMinutes(10);

    private readonly ISender _mediator;
    private readonly ILogger<JobDigestTriggerMessageConsumer> _logger;

    public JobDigestTriggerMessageConsumer(
        ISender mediator,
        ILogger<JobDigestTriggerMessageConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JobDigestTriggerMessage> context)
    {
        _logger.LogInformation("[JobDigest] Trigger received. Waiting {Delay} for match scoring to drain.", ScoringDrainDelay);

        await Task.Delay(ScoringDrainDelay, context.CancellationToken);

        await _mediator.Send(new JobDigestSendCommand(), context.CancellationToken);

        _logger.LogInformation("[JobDigest] Digest send dispatched.");
    }
}
