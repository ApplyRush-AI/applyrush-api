using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Settings;
using DTO.Profile.Settings.JobPreferences;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Settings.JobPreferences.Queries;

public sealed record JobPreferenceGetQuery : IQuery<JobPreferenceResponse>;

public sealed class JobPreferenceGetQueryHandler : IQueryHandler<JobPreferenceGetQuery, JobPreferenceResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobPreferenceGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<JobPreferenceResponse> Handle(JobPreferenceGetQuery request, CancellationToken cancellationToken)
    {
        var pref = await _dbContext.UserJobPreference
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserJobPreference>();

        return _mapper.Map<JobPreferenceResponse>(pref);
    }
}
