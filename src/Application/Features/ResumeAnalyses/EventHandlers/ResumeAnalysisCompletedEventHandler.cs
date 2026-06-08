using Application.Common.MessageBroker;
using AutoMapper;
using Domain.Events.Tailoring;
using DTO.Enums.Notification;
using DTO.MessageBroker.Messages.Notification;
using DTO.Notification.Payloads;
using MediatR;

namespace Application.Features.ResumeAnalyses.EventHandlers;

public sealed class ResumeAnalysisCompletedEventHandler : INotificationHandler<ResumeAnalysisCompletedEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    public ResumeAnalysisCompletedEventHandler(IMessagePublisher messagePublisher, IMapper mapper)
    {
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task Handle(ResumeAnalysisCompletedEvent eventData, CancellationToken cancellationToken)
    {
        var data = _mapper.Map<string>(new ResumeAnalysisNotificationPayload { AnalysisId = eventData.Analysis.Id });
        await _messagePublisher.PublishAsync(new CreateNotificationMessage(
            eventData.Analysis.UserId,
            "Resume Analysis Completed",
            "Your resume analysis results are ready.",
            data,
            NotificationType.ResumeAnalysisCompleted));
    }
}
