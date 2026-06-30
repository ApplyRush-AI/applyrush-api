using Application.Common.MessageBroker;
using Domain.Events.Profiles;
using DTO.MessageBroker.Messages.Jobs;
using MediatR;

namespace Application.Features.Profiles.EventHandlers;

public sealed class UserProfileUpdatedEventHandler : INotificationHandler<UserProfileUpdatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public UserProfileUpdatedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new JobMatchRebuildUserMessage(notification.Profile.UserId), cancellationToken);
    }
}
