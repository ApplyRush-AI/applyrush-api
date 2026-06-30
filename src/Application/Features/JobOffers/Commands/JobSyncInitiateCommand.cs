using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.JobSync;

namespace Application.Features.JobOffers.Commands;

public sealed record JobSyncInitiateCommand(int PagesPerQuery = 1) : ICommand;

public sealed class JobSyncInitiateCommandHandler : ICommandHandler<JobSyncInitiateCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public JobSyncInitiateCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(JobSyncInitiateCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new JobSyncTriggerMessage(command.PagesPerQuery), cancellationToken);
    }
}
