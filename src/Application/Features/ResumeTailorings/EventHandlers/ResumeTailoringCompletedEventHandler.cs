using Application.Common.MessageBroker;
using AutoMapper;
using Domain.Events.Tailoring;
using DTO.Enums.Notification;
using DTO.MessageBroker.Messages.Notification;
using DTO.Notification.Payloads;
using MediatR;

namespace Application.Features.ResumeTailorings.EventHandlers;

public sealed class ResumeTailoringCompletedEventHandler : INotificationHandler<ResumeTailoringCompletedEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    public ResumeTailoringCompletedEventHandler(IMessagePublisher messagePublisher, IMapper mapper)
    {
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task Handle(ResumeTailoringCompletedEvent eventData, CancellationToken cancellationToken)
    {
        var data = _mapper.Map<string>(new ResumeTailoringNotificationPayload { TailoringId = eventData.Tailoring.Id });
        await _messagePublisher.PublishAsync(new CreateNotificationMessage(
            eventData.Tailoring.UserId,
            "Resume Tailoring Completed",
            "Your tailored resume is ready.",
            data,
            NotificationType.ResumeTailoringCompleted));
    }
}
