using Application.Common.Search;
using DTO.JobOffers;

namespace Application.Features.JobOffers.Search;

public sealed record JobOfferSearchable : JobOfferFeedItemResponse, ISearchable
{
}
