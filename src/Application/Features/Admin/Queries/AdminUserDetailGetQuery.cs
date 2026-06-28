using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.User;
using DTO.Admin;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries;

public sealed record AdminUserDetailGetQuery(int UserId) : IQuery<AdminUserDetailResponse>;

public sealed class AdminUserDetailGetQueryHandler : IQueryHandler<AdminUserDetailGetQuery, AdminUserDetailResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AdminUserDetailGetQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AdminUserDetailResponse> Handle(AdminUserDetailGetQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.UserSubscription
            .AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == query.UserId, cancellationToken)
            ?? throw NotFoundException.New<ApplicationUser>();

        var resumeCount = await _dbContext.Resume.CountAsync(r => r.UserId == query.UserId, cancellationToken);
        var tailoringCount = await _dbContext.ResumeTailoring.CountAsync(t => t.UserId == query.UserId, cancellationToken);
        var analysisCount = await _dbContext.ResumeAnalysis.CountAsync(a => a.UserId == query.UserId, cancellationToken);
        var applicationCount = await _dbContext.JobApplication.CountAsync(a => a.UserId == query.UserId, cancellationToken);

        var credit = await _dbContext.UserCredit
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == query.UserId, cancellationToken);

        var tailoringRemaining = credit is null ? 0 : credit.TailoringCreditsTotal - credit.TailoringCreditsUsed;
        var analysisRemaining = credit is null ? 0 : credit.AnalysisCreditsTotal - credit.AnalysisCreditsUsed;
        var autofillRemaining = credit is null ? 0 : credit.AutofillCreditsTotal - credit.AutofillCreditsUsed;

        return _mapper.Map<AdminUserDetailResponse>(subscription) with
        {
            ResumeCount = resumeCount,
            TailoringCount = tailoringCount,
            AnalysisCount = analysisCount,
            ApplicationCount = applicationCount,
            TailoringCreditsRemaining = tailoringRemaining,
            AnalysisCreditsRemaining = analysisRemaining,
            AutofillCreditsRemaining = autofillRemaining
        };
    }
}


