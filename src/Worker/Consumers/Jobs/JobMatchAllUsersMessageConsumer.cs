using Application.Features.JobOffers.Commands;
using DTO.MessageBroker.Messages.Jobs;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Jobs;

public sealed class JobMatchAllUsersMessageConsumer : IConsumer<JobMatchAllUsersMessage>
{
    private readonly ISender _mediator;

    public JobMatchAllUsersMessageConsumer(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<JobMatchAllUsersMessage> context)
    {
        await _mediator.Send(new JobMatchAllUsersCommand(context.Message.JobId));
    }
}
