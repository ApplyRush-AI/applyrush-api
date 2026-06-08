using Application.Features.ResumeTailorings.Commands;
using DTO.MessageBroker.Messages.Tailoring;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Tailoring;

public sealed class ResumeTailoringProcessMessageConsumer : IConsumer<ResumeTailoringProcessMessage>
{
    private readonly ISender _mediator;

    public ResumeTailoringProcessMessageConsumer(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ResumeTailoringProcessMessage> context)
    {
        await _mediator.Send(new ResumeTailoringProcessCommand(context.Message.TailoringId));
    }
}
