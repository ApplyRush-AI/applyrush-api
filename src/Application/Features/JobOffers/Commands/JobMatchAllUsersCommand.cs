using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Jobs.JobListings;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Commands;

public sealed record JobMatchAllUsersCommand(int JobId) : ICommand;

public sealed class JobMatchAllUsersCommandHandler : ICommandHandler<JobMatchAllUsersCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMatchScoringService _matchScoringService;

    public JobMatchAllUsersCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMatchScoringService matchScoringService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _matchScoringService = matchScoringService;
    }

    public async Task Handle(JobMatchAllUsersCommand command, CancellationToken cancellationToken)
    {
        var jobListing = await _dbContext.JobListing
            .FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw NotFoundException.New<JobListing>();

        var existing = await _dbContext.UserJobMatch
            .Where(m => m.JobId == command.JobId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
            _dbContext.UserJobMatch.RemoveRange(existing);

        var matches = await _matchScoringService.ComputeForAllUsersAsync(command.JobId, cancellationToken);

        jobListing.AppendUserMatches(matches);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
