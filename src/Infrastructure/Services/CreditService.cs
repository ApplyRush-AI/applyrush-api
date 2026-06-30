using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Features.Credits.Data;
using Domain.Entities.Subscriptions.UserCredits;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class CreditService : ICreditService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public CreditService(IApplicationDbContext dbContext, IUnitOfWork unitOfWork, IDateTime dateTime)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<int> GetTailoringCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        return credit.TailoringCreditsTotal == -1 ? -1 : credit.TailoringCreditsTotal - credit.TailoringCreditsUsed;
    }

    public async Task<int> GetAnalysisCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        return credit.AnalysisCreditsTotal == -1 ? -1 : credit.AnalysisCreditsTotal - credit.AnalysisCreditsUsed;
    }

    public async Task<int> GetAutofillCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        return credit.AutofillCreditsTotal == -1 ? -1 : credit.AutofillCreditsTotal - credit.AutofillCreditsUsed;
    }

    public async Task DeductTailoringCreditAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        if (credit.TailoringCreditsTotal != -1 && credit.TailoringCreditsUsed >= credit.TailoringCreditsTotal)
            throw new InsufficientCreditsException();
        credit.DeductTailoring();
    }

    public async Task DeductAnalysisCreditAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        if (credit.AnalysisCreditsTotal != -1 && credit.AnalysisCreditsUsed >= credit.AnalysisCreditsTotal)
            throw new InsufficientCreditsException();
        credit.DeductAnalysis();
    }

    public async Task DeductAutofillCreditAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await GetCreditAsync(userId, cancellationToken);
        if (credit.AutofillCreditsTotal != -1 && credit.AutofillCreditsUsed >= credit.AutofillCreditsTotal)
            throw new InsufficientCreditsException();
        credit.DeductAutofill();
    }

    private async Task<UserCredit> GetCreditAsync(int userId, CancellationToken cancellationToken)
    {
        var credit = await _dbContext.UserCredit
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (credit is not null)
            return credit;

        // Back-fill for users registered before the credit system was introduced
        credit = UserCredit.Create(new UserCreditInsertData(userId, 2, 1, 4, _dateTime.Now));
        await _dbContext.UserCredit.AddAsync(credit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return credit;
    }
}

