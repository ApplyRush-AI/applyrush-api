using Application.Common.MessageBroker;
using Domain.Events.Resumes;
using DTO.MessageBroker.Messages.Jobs;
using MediatR;

namespace Application.Features.Resumes.EventHandlers;

public sealed class ResumeSetPrimaryEventHandler : INotificationHandler<ResumeSetPrimaryEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public ResumeSetPrimaryEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ResumeSetPrimaryEvent notification, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new JobMatchRebuildUserMessage(notification.Resume.UserId), cancellationToken);
    }
}
