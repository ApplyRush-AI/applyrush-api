using Application.Features.JobOffers.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuildJobOfferIndexMessageConsumer : IConsumer<RebuildJobOfferIndexMessage>
{
    private readonly ISender _mediatr;

    public RebuildJobOfferIndexMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<RebuildJobOfferIndexMessage> context)
    {
        await _mediatr.Send(new JobOfferRebuildSearchIndexCommand());
    }
}
