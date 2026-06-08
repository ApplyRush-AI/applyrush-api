using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferInitiateSearchIndexRebuildCommand : ICommand;

public sealed class JobOfferInitiateSearchIndexRebuildCommandHandler : ICommandHandler<JobOfferInitiateSearchIndexRebuildCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public JobOfferInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(JobOfferInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new RebuildJobOfferIndexMessage());
    }
}
