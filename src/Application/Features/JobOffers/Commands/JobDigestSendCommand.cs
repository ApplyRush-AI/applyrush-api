using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using Application.Features.JobOffers.Data;
using AutoMapper;
using Domain.Interfaces;
using DTO.Enums;
using DTO.MessageBroker.Messages.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Commands;

public sealed record JobDigestSendCommand : ICommand;

public sealed class JobDigestSendCommandHandler : ICommandHandler<JobDigestSendCommand>
{
    // Jobs fetched in this window are considered "new" for the digest. A sync run (~1h) plus the trigger
    // drain delay (~10-15 min) fits comfortably inside 2h, and since syncs are 3 days apart this window can
    // never bleed into a previous sync's jobs.
    private static readonly TimeSpan NewJobWindow = TimeSpan.FromHours(2);
    private const int MaxJobsPerUser = 10;

    private readonly IApplicationDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IDateTime _dateTime;
    private readonly IMapper _mapper;

    public JobDigestSendCommandHandler(
        IApplicationDbContext dbContext,
        IMessagePublisher messagePublisher,
        IDateTime dateTime,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
        _dateTime = dateTime;
        _mapper = mapper;
    }

    public async Task Handle(JobDigestSendCommand command, CancellationToken cancellationToken)
    {
        var digestsByUser = await BuildDigestsByUserAsync(cancellationToken);
        if (digestsByUser.Count == 0)
            return;

        var recipients = await LoadRecipientsAsync(digestsByUser.Keys, cancellationToken);

        foreach (var recipient in recipients)
        {
            await _messagePublisher.PublishAsync(new JobDigestEmailMessage
            {
                Email = recipient.Email,
                FirstName = recipient.FirstName,
                Jobs = digestsByUser[recipient.Id]
            }, cancellationToken);
        }
    }

    private async Task<Dictionary<int, List<JobDigestItem>>> BuildDigestsByUserAsync(CancellationToken cancellationToken)
    {
        var cutoff = _dateTime.Now - NewJobWindow;

        var jobsById = await _dbContext.JobListing
            .AsNoTracking()
            .Where(j => j.Status == Status.Active && j.Created >= cutoff)
            .ToDictionaryAsync(j => j.Id, cancellationToken);

        if (jobsById.Count == 0)
            return new();

        var jobIds = jobsById.Keys.ToList();

        var matches = await _dbContext.UserJobMatch
            .AsNoTracking()
            .Where(m => m.OverallScore > 0 && jobIds.Contains(m.JobId))
            .ToListAsync(cancellationToken);

        return matches
            .GroupBy(m => m.UserId)
            .ToDictionary(
                g => g.Key,
                g => _mapper.Map<List<JobDigestItem>>(
                    g.OrderByDescending(m => m.OverallScore)
                        .Take(MaxJobsPerUser)
                        .Select(m => new ScoredJob(jobsById[m.JobId], m.OverallScore))
                        .ToList()));
    }

    private async Task<List<DigestRecipient>> LoadRecipientsAsync(IEnumerable<int> userIds, CancellationToken cancellationToken)
    {
        return await _dbContext.User
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id) && u.Email != null)
            .Select(u => new DigestRecipient(u.Id, u.Email!, u.FirstName ?? string.Empty))
            .ToListAsync(cancellationToken);
    }
}
