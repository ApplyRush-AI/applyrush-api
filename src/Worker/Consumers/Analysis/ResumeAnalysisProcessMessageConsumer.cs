using Application.Features.ResumeAnalyses.Commands;
using DTO.MessageBroker.Messages.Analysis;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Analysis;

public sealed class ResumeAnalysisProcessMessageConsumer : IConsumer<ResumeAnalysisProcessMessage>
{
    private readonly ISender _mediator;

    public ResumeAnalysisProcessMessageConsumer(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ResumeAnalysisProcessMessage> context)
    {
        await _mediator.Send(new ResumeAnalysisProcessCommand(context.Message.AnalysisId));
    }
}
