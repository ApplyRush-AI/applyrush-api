using Application.Common.MessageBroker;
using Domain.Events.Resumes;
using DTO.MessageBroker.Messages.Jobs;
using MediatR;

namespace Application.Features.Resumes.EventHandlers;

public sealed class ResumeParsedEventHandler : INotificationHandler<ResumeParsedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public ResumeParsedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ResumeParsedEvent notification, CancellationToken cancellationToken)
    {
        // A primary resume's parsed data populates the user's profile (skills, experience, education),
        // so the user must be re-matched against all existing jobs. This closes the gap where uploading
        // the first (auto-primary) resume never triggered matching, because the upload/parse path raises
        // neither UserProfileUpdatedEvent nor ResumeSetPrimaryEvent. Non-primary resumes don't feed the
        // profile, so they don't affect matching.
        if (!notification.Resume.IsPrimary)
            return;

        await _messagePublisher.PublishAsync(new JobMatchRebuildUserMessage(notification.Resume.UserId), cancellationToken);
    }
}
