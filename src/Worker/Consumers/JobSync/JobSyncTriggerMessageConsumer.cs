using Application.Features.JobOffers.Commands;
using DTO.MessageBroker.Messages.JobSync;
using MassTransit;
using MediatR;

namespace Worker.Consumers.JobSync;

public sealed class JobSyncTriggerMessageConsumer : IConsumer<JobSyncTriggerMessage>
{
    private readonly ISender _mediatr;

    public JobSyncTriggerMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<JobSyncTriggerMessage> context)
    {
        await _mediatr.Send(new JobSyncCommand(context.Message.PagesPerQuery), context.CancellationToken);
    }
}
