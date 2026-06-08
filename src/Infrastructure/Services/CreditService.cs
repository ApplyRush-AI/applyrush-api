using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public sealed class CreditService : ICreditService
{
    // Stub: Credit tracking will be fully implemented in MS5 with the UserCredit entity.
    // For now, all users have unlimited credits (returns -1) to allow development to proceed.

    public Task<int> GetTailoringCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
        => Task.FromResult(-1);

    public Task<int> GetAnalysisCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
        => Task.FromResult(-1);

    public Task<int> GetAutofillCreditsRemainingAsync(int userId, CancellationToken cancellationToken)
        => Task.FromResult(-1);

    public Task DeductTailoringCreditAsync(int userId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task DeductAnalysisCreditAsync(int userId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task DeductAutofillCreditAsync(int userId, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
