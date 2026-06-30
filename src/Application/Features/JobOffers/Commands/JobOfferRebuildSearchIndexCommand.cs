using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.JobOffers.Queries;
using Application.Features.JobOffers.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferRebuildSearchIndexCommand : ICommand;

public sealed class JobOfferRebuildSearchIndexCommandHandler : ICommandHandler<JobOfferRebuildSearchIndexCommand>
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

    public async Task Handle(JobOfferRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<JobOfferSearchable>();

        try
        {
            _logger.LogInformation("Dropping and recreating index: {0}", index);
            await _searchClient.DeleteIndexAsync(index, cancellationToken);
            await _searchClient.CreateIndexIfNotExist(index);
            _logger.LogInformation("Index recreated: {0}", index);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while recreating Elastic index {0}", index);
        }

        try
        {
            _logger.LogInformation("Attempting to index data for index: {0}", index);

            var jobs = await _mediatr.Send(new JobOfferGetAllQuery(), cancellationToken);

            if (jobs.Any())
            {
                var searchableJobs = _mapper.Map<IReadOnlyCollection<JobOfferSearchable>>(jobs);
                await _searchClient.IndexAndRefreshManyAsync(searchableJobs, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No job offers to index...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}
