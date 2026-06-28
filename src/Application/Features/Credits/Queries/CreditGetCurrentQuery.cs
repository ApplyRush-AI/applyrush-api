using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Subscriptions.UserCredits;
using Domain.Interfaces;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Credits.Queries;

public sealed record CreditGetCurrentQuery : IQuery<UserCreditResponse>;

public sealed class CreditGetCurrentQueryHandler : IQueryHandler<CreditGetCurrentQuery, UserCreditResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTime _dateTimeProvider;

    public CreditGetCurrentQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IDateTime dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UserCreditResponse> Handle(CreditGetCurrentQuery query, CancellationToken cancellationToken)
    {
        var credit = await _dbContext.UserCredit
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserCredit>();

        static int Remaining(int total, int used) => total == -1 ? -1 : total - used;

        var resetsAt = _dateTimeProvider.Now.Date.AddDays(1);

        return new UserCreditResponse(
            new CreditTypeUsageResponse(
                credit.TailoringCreditsUsed,
                Remaining(credit.TailoringCreditsTotal, credit.TailoringCreditsUsed),
                credit.TailoringCreditsTotal),
            new CreditTypeUsageResponse(
                credit.AnalysisCreditsUsed,
                Remaining(credit.AnalysisCreditsTotal, credit.AnalysisCreditsUsed),
                credit.AnalysisCreditsTotal),
            new CreditTypeUsageResponse(
                credit.AutofillCreditsUsed,
                Remaining(credit.AutofillCreditsTotal, credit.AutofillCreditsUsed),
                credit.AutofillCreditsTotal),
            resetsAt
        );
    }
}

