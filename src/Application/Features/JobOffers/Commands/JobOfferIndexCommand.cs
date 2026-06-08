using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.JobOffers.Queries;
using Application.Features.JobOffers.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferIndexCommand(int JobId) : ICommand;

public sealed class JobOfferIndexCommandHandler : ICommandHandler<JobOfferIndexCommand>
{
    private readonly ILogger<JobOfferIndexCommandHandler> _logger;
    private readonly ISearchClient<JobOfferSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobOfferIndexCommandHandler(
        ILogger<JobOfferIndexCommandHandler> logger,
        ISearchClient<JobOfferSearchable> searchClient,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    public async Task Handle(JobOfferIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for job offer with ID: {0}", command.JobId);
        var job = await _mediatr.Send(new JobOfferGetByIdQuery(command.JobId), cancellationToken);

        if (job != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<JobOfferSearchable>(job), cancellationToken);
                _logger.LogInformation("Indexing finished for job offer with ID: {0}", command.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for job offer with ID: {0}", command.JobId);
            }
        }
        else
        {
            _logger.LogInformation("Job offer does not exist: {0}", command.JobId);
        }
    }
}
