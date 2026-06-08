using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using DTO.JobOffers;

namespace Application.Features.JobOffers.Queries;

public sealed record JobSyncStatusGetQuery : IQuery<JobOfferSyncStatusResponse>;

public sealed class JobSyncStatusGetQueryHandler : IQueryHandler<JobSyncStatusGetQuery, JobOfferSyncStatusResponse>
{
    private readonly IJobSyncState _syncState;

    public JobSyncStatusGetQueryHandler(IJobSyncState syncState)
    {
        _syncState = syncState;
    }

    public async Task<JobOfferSyncStatusResponse> Handle(JobSyncStatusGetQuery query, CancellationToken cancellationToken)
    {
        var state = await _syncState.GetAsync(cancellationToken);

        return new JobOfferSyncStatusResponse
        {
            Status = state.Status,
            LastSyncedAt = state.LastSyncedAt,
            JobsIndexed = state.JobsIndexed,
            ErrorMessage = state.ErrorMessage
        };
    }
}
