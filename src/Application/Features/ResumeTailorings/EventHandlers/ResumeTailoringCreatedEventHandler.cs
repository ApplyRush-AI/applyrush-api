using Application.Common.MessageBroker;
using Domain.Events.Tailoring;
using DTO.MessageBroker.Messages.Tailoring;
using MediatR;

namespace Application.Features.ResumeTailorings.EventHandlers;

public sealed class ResumeTailoringCreatedEventHandler : INotificationHandler<ResumeTailoringCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public ResumeTailoringCreatedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ResumeTailoringCreatedEvent eventData, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new ResumeTailoringProcessMessage(
            eventData.Tailoring.Id,
            eventData.Tailoring.UserId));
    }
}
