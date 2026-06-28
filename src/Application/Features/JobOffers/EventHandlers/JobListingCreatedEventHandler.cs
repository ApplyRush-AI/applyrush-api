using Application.Features.JobOffers.Commands;
using Domain.Events.Jobs;
using MediatR;

namespace Application.Features.JobOffers.EventHandlers;

public sealed class JobListingCreatedEventHandler : INotificationHandler<JobListingCreatedEvent>
{
    private readonly ISender _mediator;

    public JobListingCreatedEventHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(JobListingCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new JobOfferIndexCommand(notification.JobListing.Id), cancellationToken);
    }
}
