using Application.Common.MessageBroker;
using Domain.Events.Jobs;
using DTO.MessageBroker.Messages.Jobs;
using MediatR;

namespace Application.Features.JobOffers.EventHandlers;

public sealed class JobMatchAllUsersEventHandler : INotificationHandler<JobListingCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public JobMatchAllUsersEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(JobListingCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new JobMatchAllUsersMessage(notification.JobListing.Id));
    }
}
