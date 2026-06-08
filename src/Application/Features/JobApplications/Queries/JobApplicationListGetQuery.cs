using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums.JobApplication;
using DTO.JobApplications;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobApplications.Queries;

public sealed record JobApplicationListGetQuery : IQuery<IReadOnlyList<JobApplicationResponse>>;

public sealed class JobApplicationListGetQueryHandler : IQueryHandler<JobApplicationListGetQuery, IReadOnlyList<JobApplicationResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobApplicationListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<JobApplicationResponse>> Handle(JobApplicationListGetQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var applications = await _dbContext.JobApplication
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Status != ApplicationStatus.Deleted)
            .Include(a => a.Job)
                .ThenInclude(j => j.UserMatches.Where(m => m.UserId == userId))
            .Include(a => a.Job)
                .ThenInclude(j => j.SavedByUsers.Where(s => s.UserId == userId))
            .OrderByDescending(a => a.Created)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<JobApplicationResponse>>(applications);
    }
}
