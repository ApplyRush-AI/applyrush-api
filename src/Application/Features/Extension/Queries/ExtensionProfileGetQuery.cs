using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums;
using DTO.Extension;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Extension.Queries;

public sealed record ExtensionProfileGetQuery : IQuery<ExtensionProfileResponse>;

public sealed class ExtensionProfileGetQueryHandler : IQueryHandler<ExtensionProfileGetQuery, ExtensionProfileResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ExtensionProfileGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ExtensionProfileResponse> Handle(ExtensionProfileGetQuery query, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .AsNoTracking()
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences.Where(w => w.Status != Status.Deleted).OrderBy(w => w.OrderIndex))
                .ThenInclude(w => w.Bullets.OrderBy(b => b.OrderIndex))
            .Include(p => p.Educations.Where(e => e.Status != Status.Deleted).OrderBy(e => e.OrderIndex))
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        return _mapper.Map<ExtensionProfileResponse>(profile);
    }
}
