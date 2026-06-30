using Domain.Entities.Jobs.JobListings;

namespace Application.Features.JobOffers.Data;

internal sealed record ScoredJob(JobListing Job, decimal Score);
