using Application.Features.JobOffers.Commands;
using Domain.Events.Jobs;
using MediatR;

namespace Application.Features.JobOffers.EventHandlers;

public sealed class JobUserMatchesComputedEventHandler : INotificationHandler<JobUserMatchesComputedEvent>
{
    private readonly ISender _mediator;

    public JobUserMatchesComputedEventHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(JobUserMatchesComputedEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new JobOfferIndexCommand(notification.JobListing.Id), cancellationToken);
    }
}
