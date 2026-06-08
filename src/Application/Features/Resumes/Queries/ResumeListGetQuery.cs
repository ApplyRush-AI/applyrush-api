using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Queries;

public sealed record ResumeListGetQuery : IQuery<IReadOnlyList<ResumeListItemResponse>>;

public sealed class ResumeListGetQueryHandler : IQueryHandler<ResumeListGetQuery, IReadOnlyList<ResumeListItemResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ResumeListItemResponse>> Handle(ResumeListGetQuery request, CancellationToken cancellationToken)
    {
        var resumes = await _dbContext.Resume
            .Where(r => r.UserId == _currentUserService.UserId!.Value && r.Status == Status.Active)
            .OrderByDescending(r => r.IsPrimary)
            .ThenByDescending(r => r.Created)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ResumeListItemResponse>>(resumes);
    }
}
