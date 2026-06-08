using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Resumes.Resume;
using DTO.Enums;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Queries;

public sealed record ResumeGetByIdQuery(int ResumeId) : IQuery<ResumeDetailResponse>;

public sealed class ResumeGetByIdQueryHandler : IQueryHandler<ResumeGetByIdQuery, ResumeDetailResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResumeGetByIdQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ResumeDetailResponse> Handle(ResumeGetByIdQuery request, CancellationToken cancellationToken)
    {
        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(
                r => r.Id == request.ResumeId &&
                     r.UserId == _currentUserService.UserId!.Value &&
                     r.Status == Status.Active,
                cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        return _mapper.Map<ResumeDetailResponse>(resume);
    }
}
