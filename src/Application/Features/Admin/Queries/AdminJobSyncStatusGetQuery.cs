using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using AutoMapper;
using DTO.Admin;

namespace Application.Features.Admin.Queries;

public sealed record AdminJobSyncStatusGetQuery : IQuery<AdminJobSyncStatusResponse>;

public sealed class AdminJobSyncStatusGetQueryHandler : IQueryHandler<AdminJobSyncStatusGetQuery, AdminJobSyncStatusResponse>
{
    private readonly IJobSyncState _jobSyncState;
    private readonly IMapper _mapper;

    public AdminJobSyncStatusGetQueryHandler(IJobSyncState jobSyncState, IMapper mapper)
    {
        _jobSyncState = jobSyncState;
        _mapper = mapper;
    }

    public async Task<AdminJobSyncStatusResponse> Handle(AdminJobSyncStatusGetQuery query, CancellationToken cancellationToken)
    {
        var state = await _jobSyncState.GetAsync(cancellationToken);
        return _mapper.Map<AdminJobSyncStatusResponse>(state);
    }
}

