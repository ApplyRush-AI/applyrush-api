using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.JobOffers.Queries;
using Application.Features.JobOffers.Search;
using AutoMapper;
using DTO.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferRebuildSearchIndexCommand : ICommand<SearchIndexRebuildResult>;

public sealed class JobOfferRebuildSearchIndexCommandHandler : ICommandHandler<JobOfferRebuildSearchIndexCommand, SearchIndexRebuildResult>
{
    private readonly ILogger<JobOfferRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<JobOfferSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobOfferRebuildSearchIndexCommandHandler(
        ILogger<JobOfferRebuildSearchIndexCommandHandler> logger,
        ISearchClient<JobOfferSearchable> searchClient,
        ISearchIndexProvider searchIndexProvider,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _searchIndexProvider = searchIndexProvider;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    // Every failure is reported back to the caller instead of being logged and swallowed: a rebuild that
    // silently does nothing looks identical to one that worked, which is how an empty job feed went unnoticed.
    public async Task<SearchIndexRebuildResult> Handle(JobOfferRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<JobOfferSearchable>();

        if (!await _searchClient.IsAvailableAsync())
        {
            const string error = "Elasticsearch did not respond. The index was not rebuilt and job search will keep returning no results until the cluster is reachable again.";
            _logger.LogError("Rebuild aborted for index {Index}: Elasticsearch unreachable", index);
            return new SearchIndexRebuildResult { Succeeded = false, Index = index, Error = error };
        }

        try
        {
            _logger.LogInformation("Dropping and recreating index {Index}", index);
            await _searchClient.DeleteIndexAsync(index, cancellationToken);

            if (!await _searchClient.CreateIndexIfNotExist(index))
            {
                const string error = "Elasticsearch refused to create the index. This usually means the cluster is read-only (a full disk is the common cause) or the credentials cannot create indices.";
                _logger.LogError("Failed to create index {Index}", index);
                return new SearchIndexRebuildResult { Succeeded = false, Index = index, Error = error };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while recreating index {Index}", index);
            return new SearchIndexRebuildResult { Succeeded = false, Index = index, Error = $"Could not recreate the index: {ex.Message}" };
        }

        try
        {
            var jobs = await _mediatr.Send(new JobOfferGetAllQuery(), cancellationToken);

            if (!jobs.Any())
            {
                _logger.LogInformation("No job offers to index for {Index}", index);
                return new SearchIndexRebuildResult { Succeeded = true, Index = index, DocumentsIndexed = 0 };
            }

            var searchableJobs = _mapper.Map<IReadOnlyCollection<JobOfferSearchable>>(jobs);
            await _searchClient.IndexAndRefreshManyAsync(searchableJobs, cancellationToken);

            _logger.LogInformation("Indexed {Count} job offers into {Index}", searchableJobs.Count, index);
            return new SearchIndexRebuildResult { Succeeded = true, Index = index, DocumentsIndexed = searchableJobs.Count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while indexing data into {Index}", index);
            return new SearchIndexRebuildResult { Succeeded = false, Index = index, Error = $"The index was recreated but indexing the job offers failed: {ex.Message}" };
        }
    }
}
