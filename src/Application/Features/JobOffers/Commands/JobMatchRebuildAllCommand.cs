using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Jobs;

namespace Application.Features.JobOffers.Commands;

public sealed record JobMatchRebuildAllCommand : ICommand;

public sealed class JobMatchRebuildAllCommandHandler : ICommandHandler<JobMatchRebuildAllCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public JobMatchRebuildAllCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(JobMatchRebuildAllCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new JobMatchRebuildAllMessage(), cancellationToken);
    }
}
