using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Settings;
using DTO.Profile.Settings.NotificationPreferences;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Settings.NotificationPreferences.Queries;

public sealed record NotificationPreferenceGetQuery : IQuery<NotificationPreferenceResponse>;

public sealed class NotificationPreferenceGetQueryHandler : IQueryHandler<NotificationPreferenceGetQuery, NotificationPreferenceResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public NotificationPreferenceGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<NotificationPreferenceResponse> Handle(NotificationPreferenceGetQuery request, CancellationToken cancellationToken)
    {
        var pref = await _dbContext.UserNotificationPreference
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserNotificationPreference>();

        return _mapper.Map<NotificationPreferenceResponse>(pref);
    }
}
