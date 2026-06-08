using Application.Common.MessageBroker;
using Domain.Events.Tailoring;
using DTO.MessageBroker.Messages.Analysis;
using MediatR;

namespace Application.Features.ResumeAnalyses.EventHandlers;

public sealed class ResumeAnalysisCreatedEventHandler : INotificationHandler<ResumeAnalysisCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public ResumeAnalysisCreatedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ResumeAnalysisCreatedEvent eventData, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new ResumeAnalysisProcessMessage(
            eventData.Analysis.Id,
            eventData.Analysis.UserId));
    }
}
