using Domain.Entities.Subscriptions.UserCredits;

namespace Application.Common.Interfaces.Services;

public interface ICreditService
{
    /// <summary>
    /// Returns the user's credit record, back-filling a default free-tier row if one does not exist yet.
    /// This is the single source of truth used by both the read (credits screen) and spend paths, so a user
    /// who opens the credits screen before ever spending a credit gets a row instead of a 404.
    /// </summary>
    Task<UserCredit> GetOrCreateCreditAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the number of remaining tailoring credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetTailoringCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the number of remaining analysis credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetAnalysisCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the number of remaining autofill credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetAutofillCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one tailoring credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductTailoringCreditAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one analysis credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductAnalysisCreditAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one autofill credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductAutofillCreditAsync(int userId, CancellationToken cancellationToken);
}
